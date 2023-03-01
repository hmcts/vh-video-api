using Microsoft.AspNetCore.Mvc.Testing;
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
        protected WebApplicationFactory<Program> Application = null!;
        protected TestDataManager TestDataManager = null!;
        protected DbContextOptions<VideoApiDbContext> DbOptions { get; private set; }
        private IConfigurationRoot _configRoot;
        private string _databaseConnectionString;
        private KinlyConfiguration _kinlyConfiguration;
        private ServicesConfiguration _services;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            RegisterSettings();
            Application = new VhApiWebApplicationFactory();
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
            _configRoot = ConfigRootBuilder.Build();
            _services = _configRoot.GetSection("Services").Get<ServicesConfiguration>();
            _kinlyConfiguration = _configRoot.GetSection("KinlyConfiguration").Get<KinlyConfiguration>();
            _kinlyConfiguration.CallbackUri = _services.CallbackUri;
            _databaseConnectionString = _configRoot.GetConnectionString("VideoApi");
        }
    }
}
