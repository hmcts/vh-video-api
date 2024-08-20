using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Testing.Common.Configuration;
using VideoApi.Common.Configuration;
using VideoApi.Common.Security.Supplier.Base;
using VideoApi.Common.Security.Supplier.Kinly;
using VideoApi.Common.Security.Supplier.Vodafone;
using VideoApi.Contract.Responses;
using VideoApi.DAL;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.IntegrationTests.Helper;
using VideoApi.Mappings;

namespace VideoApi.IntegrationTests.Api.Setup
{
    public class ApiTest
    {
        private string _databaseConnectionString;
        private KinlyConfiguration _kinlyConfiguration;
        private ServicesConfiguration _services;
        private VodafoneConfiguration _vodafoneConfiguration;
        protected VhApiWebApplicationFactory Application = null!;
        protected IConfigurationRoot ConfigRoot;
        protected TestDataManager TestDataManager = null!;
        protected DbContextOptions<VideoApiDbContext> DbOptions { get; private set; }
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            RegisterSettings();
            Application = new VhApiWebApplicationFactory(ConfigRoot);
            InitTestDataManager();
        }
        
        private void InitTestDataManager()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<VideoApiDbContext>();
            dbContextOptionsBuilder.UseSqlServer(_databaseConnectionString);
            DbOptions = dbContextOptionsBuilder.Options;
            
            var context = new VideoApiDbContext(DbOptions);
            context.Database.Migrate();
            
            TestDataManager = new TestDataManager(_kinlyConfiguration, dbContextOptionsBuilder.Options,
                _vodafoneConfiguration);
        }
        
        private void RegisterSettings()
        {
            ConfigRoot = ConfigRootBuilder.Build();
            _services = ConfigRoot.GetSection("Services").Get<ServicesConfiguration>();
            _kinlyConfiguration = ConfigRoot.GetSection("KinlyConfiguration").Get<KinlyConfiguration>();
            _kinlyConfiguration.CallbackUri = _services.CallbackUri;
            _vodafoneConfiguration = ConfigRoot.GetSection("VodafoneConfiguration").Get<VodafoneConfiguration>();
            _vodafoneConfiguration.CallbackUri = _services.CallbackUri;
            _databaseConnectionString = ConfigRoot.GetConnectionString("VideoApi");
        }
        
        protected void VerifyConferenceInResponse(ConferenceDetailsResponse conferenceInResponse,
            Conference conferenceInDb)
        {
            var kinlyConfiguration = TestDataManager.GetKinlyConfiguration();
            var vodafoneConfiguration = TestDataManager.GetVodafoneConfiguration();
            
            conferenceInResponse.Should().NotBeNull();
            conferenceInResponse.Id.Should().Be(conferenceInDb.Id);
            SupplierConfiguration supplierConfiguration =
                conferenceInDb.Supplier == Supplier.Vodafone ? vodafoneConfiguration : kinlyConfiguration;
            conferenceInResponse.Should().BeEquivalentTo(
                ConferenceToDetailsResponseMapper.Map(conferenceInDb, supplierConfiguration),
                options => options.Excluding(x => x.Participants));
        }
    }
}
