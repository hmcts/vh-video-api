using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace RefData
{
    [ExcludeFromCodeCoverage]
    public class DesignTimeHearingsContextFactory : IDesignTimeDbContextFactory<RefDataContext>
    {
        public RefDataContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .AddUserSecrets("9AECE566-336D-4D16-88FA-7A76C27321CD")
                .AddEnvironmentVariables()
                .Build();
            var builder = new DbContextOptionsBuilder<RefDataContext>();
            builder.UseSqlServer(config.GetConnectionString("VideoApi"));
            var context = new RefDataContext(builder.Options);
            return context;
        }
    }

}