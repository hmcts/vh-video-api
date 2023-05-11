using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Testing.Common.Configuration;
using VideoApi.Common.Configuration;
using VideoApi.Common.Security.Kinly;
using VideoApi.DAL;
using VideoApi.IntegrationTests.Helper;

namespace VideoApi.IntegrationTests.Api.Setup
{
    public class ApiTest
    {
        protected VhApiWebApplicationFactory Application = null!;
        protected TestDataManager TestDataManager = null!;
        protected DbContextOptions<VideoApiDbContext> DbOptions { get; private set; }
        protected IConfigurationRoot ConfigRoot;
        private string _databaseConnectionString;
        private KinlyConfiguration _kinlyConfiguration;
        private ServicesConfiguration _services;

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

            TestDataManager = new TestDataManager(_kinlyConfiguration, dbContextOptionsBuilder.Options);
        }

        private void RegisterSettings()
        {
            ConfigRoot = ConfigRootBuilder.Build();
            _services = ConfigRoot.GetSection("Services").Get<ServicesConfiguration>();
            _kinlyConfiguration = ConfigRoot.GetSection("KinlyConfiguration").Get<KinlyConfiguration>();
            _kinlyConfiguration.CallbackUri = _services.CallbackUri;
            _databaseConnectionString = ConfigRoot.GetConnectionString("VideoApi");
        }
    }
}
