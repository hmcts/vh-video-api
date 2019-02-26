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
                .AddUserSecrets("9AECE566-336D-4D16-88FA-7A76C27321CD")
                .Build();
            var builder = new DbContextOptionsBuilder<VideoApiDbContext>();
            builder.UseSqlServer(config.GetConnectionString("VhVideoApi"));
            var context = new VideoApiDbContext(builder.Options);
            return context;
        }
    }

}