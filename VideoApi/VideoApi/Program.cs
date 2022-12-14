using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VH.Core.Configuration;
using VideoApi.DAL.Commands.Core;

namespace VideoApi
{
    public class Program
    {
        protected Program()
        {
        }

        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            CreateScope(host);
            
            
            host.Run();
        }

        private static void CreateScope(IHost host)
        {
            // var services = host.Services;
            // var handler = services.GetService<ICommandHandler>();
            
        }
        
        // ReSharper disable once MemberCanBePrivate.Global Needed for client generation on build with nswag
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            const string vhInfraCore = "/mnt/secrets/vh-infra-core";
            const string vhVideoApi = "/mnt/secrets/vh-video-api";

            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((configBuilder) =>
                {
                    configBuilder.AddAksKeyVaultSecretProvider(vhInfraCore);
                    configBuilder.AddAksKeyVaultSecretProvider(vhVideoApi);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                    webBuilder.UseIISIntegration();
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
