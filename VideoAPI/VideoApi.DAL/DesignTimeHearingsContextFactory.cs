using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace VideoApi.DAL
{
    public class DesignTimeHearingsContextFactory : IDesignTimeDbContextFactory<VideoApiDbContext>
    {
        public VideoApiDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddUserSecrets("d76b6eb8-f1a2-4a51-9b8f-21e1b6b81e4f")
                .Build();
            var builder = new DbContextOptionsBuilder<VideoApiDbContext>();
            builder.UseSqlServer(config.GetConnectionString("VhVideoApi"));
            var context = new VideoApiDbContext(builder.Options);
            return context;
        }
    }

}