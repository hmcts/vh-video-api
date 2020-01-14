using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

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

        private static IHostBuilder CreateWebHostBuilder(string[] args)
        {
           return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { 
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseKestrel(x => x.AddServerHeader = false);
                });
        }
    }
}