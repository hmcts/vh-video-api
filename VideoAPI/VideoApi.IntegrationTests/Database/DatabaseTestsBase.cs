using Video.API;
using VideoApi.DAL;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using VideoApi.IntegrationTests.Helper;

namespace VideoApi.IntegrationTests.Database
{
    public abstract class DatabaseTestsBase
    {
        private string _databaseConnectionString;
        protected DbContextOptions<VideoApiDbContext> VideoBookingsDbContextOptions;
        protected readonly BuilderSettings BuilderSettings = new BuilderSettings();
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

            var dbContextOptionsBuilder = new DbContextOptionsBuilder<VideoApiDbContext>();
            dbContextOptionsBuilder.EnableSensitiveDataLogging();
            dbContextOptionsBuilder.UseSqlServer(_databaseConnectionString);
            dbContextOptionsBuilder.EnableSensitiveDataLogging();
            VideoBookingsDbContextOptions = dbContextOptionsBuilder.Options;
            
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            context.Database.Migrate();
            
            TestDataManager = new TestDataManager(VideoBookingsDbContextOptions);
        }
    }
}