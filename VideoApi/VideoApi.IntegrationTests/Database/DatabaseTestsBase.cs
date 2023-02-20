using VideoApi.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Testing.Common.Configuration;
using VideoApi.Common.Security.Kinly;
using VideoApi.IntegrationTests.Helper;

namespace VideoApi.IntegrationTests.Database
{
    public abstract class DatabaseTestsBase
    {
        // private string _databaseConnectionString;
        private KinlyConfiguration _kinlyConfiguration;
        protected DbContextOptions<VideoApiDbContext> VideoBookingsDbContextOptions;
        protected TestDataManager TestDataManager;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var configRoot = ConfigRootBuilder.Build();
            
            _kinlyConfiguration = Options.Create(configRoot.GetSection("KinlyConfiguration").Get<KinlyConfiguration>()).Value;

            var dbContextOptionsBuilder = new DbContextOptionsBuilder<VideoApiDbContext>();
            dbContextOptionsBuilder.UseInMemoryDatabase("InMemoryDbForTesting");
            VideoBookingsDbContextOptions = dbContextOptionsBuilder.Options;

            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            if (context.Database.IsRelational())
            {
                context.Database.Migrate();
            }

            TestDataManager = new TestDataManager(_kinlyConfiguration, VideoBookingsDbContextOptions);
        }
    }
}
