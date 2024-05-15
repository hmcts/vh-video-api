using Microsoft.EntityFrameworkCore;
using VideoApi.DAL;

namespace VideoApi.UnitTests.DAL.Commands
{
    public class EfCoreSetup
    {
        protected VideoApiDbContext videoApiDbContext;

        [OneTimeSetUp]
        public void InitialSetUp()
        {
            videoApiDbContext = new VideoApiDbContext(new DbContextOptionsBuilder<VideoApiDbContext>()
                .UseInMemoryDatabase("Test")
                .Options);
        }

        [OneTimeTearDown]
        public void FinalCleanUp()
        {
            videoApiDbContext.Database.EnsureDeleted();
        }
    }
}
