using System.Linq;
using System.Net.Http;
using GST.Fake.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testing.Common;
using VideoApi.Common.Configuration;
using VideoApi.Services;
using VideoApi.Services.Contracts;

namespace VideoApi.IntegrationTests.Api.Setup
{
    public class VhApiWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly IConfigurationRoot _configRoot;

        public VhApiWebApplicationFactory(IConfigurationRoot configRoot)
        {
            _configRoot = configRoot;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = FakeJwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = FakeJwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = FakeJwtBearerDefaults.AuthenticationScheme;
                }).AddFakeJwtBearer();

                RegisterAzuriteStorageService(services);
                RegisterStubs(services);
            });
            builder.UseEnvironment("Development");
        }

        private void RegisterAzuriteStorageService(IServiceCollection services)
        {
            // Remove application existing IAzureStorageService service and replace with azurite one
            var azStorageServices = services.Where(d => d.ServiceType == typeof(IAzureStorageService)).ToList();
            foreach (var azStorageService in azStorageServices)
            {
                services.Remove(azStorageService);
            }
            var azureStorageConnectionString = _configRoot.GetValue<string>("Azure:StorageConnectionString");
            var serviceClient = AzureStorageManager.CreateAzuriteBlobServiceClient(azureStorageConnectionString);
            var blobClientExtension = new BlobClientExtension();

            var wowzaConfiguration = _configRoot.GetSection("WowzaConfiguration").Get<WowzaConfiguration>();
            var cvpConfiguration = _configRoot.GetSection("CvpConfiguration").Get<CvpConfiguration>();
            
            services.AddSingleton<IAzureStorageService>(_ =>
                new VhAzureStorageService(serviceClient, wowzaConfiguration, false, blobClientExtension));
            services.AddSingleton<IAzureStorageService>(_ =>
                new CvpAzureStorageService(serviceClient, cvpConfiguration, false, blobClientExtension));
        }
        
        private static void RegisterStubs(IServiceCollection services)
        {
            services.AddScoped<IVideoPlatformService, SupplierPlatformServiceStub>();
            services.AddScoped<IAudioPlatformService, AudioPlatformServiceStub>();
            services.AddScoped<IConsultationService, ConsultationServiceStub>();
            services.AddScoped<IVirtualRoomService, VirtualRoomServiceStub>();
            services.AddScoped<ISupplierApiSelector, SupplierApiSelectorStub>();
        }

        protected override void ConfigureClient(HttpClient client)
        {
            base.ConfigureClient(client);
            client.SetFakeBearerToken("admin", new[] { "ROLE_ADMIN", "ROLE_GENTLEMAN" });
        }
        
    }
}
