using Video.API;
using VideoApi.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using VideoApi.Common.Configuration;
using VideoApi.IntegrationTests.Helper;

namespace VideoApi.IntegrationTests.Database
{
    public abstract class DatabaseTestsBase
    {
        private string _databaseConnectionString;
        private ServicesConfiguration _services;
        protected DbContextOptions<VideoApiDbContext> VideoBookingsDbContextOptions;
        protected TestDataManager TestDataManager;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var configRootBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddUserSecrets<Startup>();
            
            var configRoot = configRootBuilder.Build();
            _databaseConnectionString = configRoot.GetConnectionString("VhVideoApi");
            _services = Options.Create(configRoot.GetSection("Services").Get<ServicesConfiguration>()).Value;

            var dbContextOptionsBuilder = new DbContextOptionsBuilder<VideoApiDbContext>();
            dbContextOptionsBuilder.EnableSensitiveDataLogging();
            dbContextOptionsBuilder.UseSqlServer(_databaseConnectionString);
            dbContextOptionsBuilder.EnableSensitiveDataLogging();
            VideoBookingsDbContextOptions = dbContextOptionsBuilder.Options;
            
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            context.Database.Migrate();
            
            TestDataManager = new TestDataManager(_services, VideoBookingsDbContextOptions);
        }
    }
}