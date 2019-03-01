using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Video.API
{
    public class Program
    {
        protected Program()
        {
        }

        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel(x => x.AddServerHeader = false)
                .UseApplicationInsights()
                .UseStartup<Startup>();
        }
    }
}