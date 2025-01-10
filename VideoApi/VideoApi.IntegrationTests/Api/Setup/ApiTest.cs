using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Testing.Common.Configuration;
using VideoApi.Common.Configuration;
using VideoApi.Common.Security.Supplier.Vodafone;
using VideoApi.DAL;
using VideoApi.IntegrationTests.Helper;

namespace VideoApi.IntegrationTests.Api.Setup
{
    public class ApiTest
    {
        private string _databaseConnectionString;
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
            
            TestDataManager = new TestDataManager(dbContextOptionsBuilder.Options, _vodafoneConfiguration);
        }
        
        private void RegisterSettings()
        {
            ConfigRoot = ConfigRootBuilder.Build();
            _services = ConfigRoot.GetSection("Services").Get<ServicesConfiguration>();
            _vodafoneConfiguration = ConfigRoot.GetSection("VodafoneConfiguration").Get<VodafoneConfiguration>();
            _vodafoneConfiguration.CallbackUri = _services.CallbackUri;
            _databaseConnectionString = ConfigRoot.GetConnectionString("VideoApi");
        }
    }
}
