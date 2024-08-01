using VideoApi.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Testing.Common.Configuration;
using VideoApi.Common.Security.Supplier.Kinly;
using VideoApi.Common.Security.Supplier.Vodafone;
using VideoApi.IntegrationTests.Helper;

namespace VideoApi.IntegrationTests.Database
{
    public abstract class DatabaseTestsBase
    {
        private string _databaseConnectionString;
        private KinlyConfiguration _kinlyConfiguration;
        private VodafoneConfiguration _vodafoneConfiguration;
        protected DbContextOptions<VideoApiDbContext> VideoBookingsDbContextOptions;
        protected TestDataManager TestDataManager;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var configRoot = ConfigRootBuilder.Build();
            _kinlyConfiguration = configRoot.GetSection("KinlyConfiguration").Get<KinlyConfiguration>();
            _vodafoneConfiguration = configRoot.GetSection("VodafoneConfiguration").Get<VodafoneConfiguration>();
            
            _databaseConnectionString = configRoot.GetConnectionString("VideoApi");
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<VideoApiDbContext>();
            dbContextOptionsBuilder.UseSqlServer(_databaseConnectionString);
            
            VideoBookingsDbContextOptions = dbContextOptionsBuilder.Options;
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            context.Database.Migrate();

            TestDataManager = new TestDataManager(_kinlyConfiguration, VideoBookingsDbContextOptions, _vodafoneConfiguration);
        }
    }
}
