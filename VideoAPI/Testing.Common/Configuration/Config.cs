using Microsoft.EntityFrameworkCore;
using VideoApi.Common.Configuration;
using VideoApi.DAL;

namespace Testing.Common.Configuration
{
    public class Config
    {
        public AzureAdConfiguration AzureAdConfiguration { get; set; }
        public ConnectionStringsConfig DbConnection { get; set; }
        public ServicesConfiguration VhServices { get; set; }
        public DbContextOptions<VideoApiDbContext> VideoBookingsDbContextOptions { get; set; }
        public ZapConfiguration ZapConfig { get; set; }
    }
}
