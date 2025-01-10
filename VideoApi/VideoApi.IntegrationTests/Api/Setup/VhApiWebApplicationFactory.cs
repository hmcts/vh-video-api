using System.Linq;
using System.Net.Http;
using GST.Fake.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Testing.Common;
using VideoApi.Common.Configuration;
using VideoApi.Common.Security.Supplier.Vodafone;
using VideoApi.Services;
using VideoApi.Services.Contracts;

namespace VideoApi.IntegrationTests.Api.Setup
{
    public class VhApiWebApplicationFactory(IConfigurationRoot configRoot) : WebApplicationFactory<Program>
    {
        private static readonly string[] Roles = ["ROLE_ADMIN", "ROLE_GENTLEMAN"];

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
            var azureStorageConnectionString = configRoot.GetValue<string>("Azure:StorageConnectionString");
            var serviceClient = AzureStorageManager.CreateAzuriteBlobServiceClient(azureStorageConnectionString);
            var blobClientExtension = new BlobClientExtension();

            var wowzaConfiguration = configRoot.GetSection("WowzaConfiguration").Get<WowzaConfiguration>();
            var cvpConfiguration = configRoot.GetSection("CvpConfiguration").Get<CvpConfiguration>();
            
            services.AddSingleton<IAzureStorageService>(_ =>
                new VhAzureStorageService(serviceClient, wowzaConfiguration, false, blobClientExtension));
            services.AddSingleton<IAzureStorageService>(_ =>
                new CvpAzureStorageService(serviceClient, cvpConfiguration, false, blobClientExtension));
        }
        
        private static void RegisterStubs(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            
            var vodafoneConfigOptions = serviceProvider.GetService<IOptions<VodafoneConfiguration>>();
            var supplierPlatformService = new SupplierPlatformServiceStub(vodafoneConfigOptions.Value);
            services.AddScoped<IVideoPlatformService>(_ => supplierPlatformService);
            services.AddScoped<IAudioPlatformService, AudioPlatformServiceStub>();
            services.AddScoped<IConsultationService, ConsultationServiceStub>();
            services.AddScoped<IVirtualRoomService, VirtualRoomServiceStub>();
        }

        protected override void ConfigureClient(HttpClient client)
        {
            base.ConfigureClient(client);
            client.SetFakeBearerToken("admin", Roles);
        }
        
    }
}
