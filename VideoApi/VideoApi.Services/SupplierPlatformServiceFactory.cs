using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VideoApi.Common.Security.Supplier.Vodafone;
using VideoApi.Services.Clients;
using VideoApi.Services.Contracts;
using Supplier = VideoApi.Domain.Enums.Supplier;

namespace VideoApi.Services
{
    public interface ISupplierPlatformServiceFactory
    {
        IVideoPlatformService Create(Supplier supplier);
    }
    
    public class SupplierPlatformServiceFactory(IServiceProvider serviceProvider) : ISupplierPlatformServiceFactory
    {
        public IVideoPlatformService Create(Supplier supplier)
        {
            var featureToggles = serviceProvider.GetRequiredService<IFeatureToggles>();
            var logger = serviceProvider.GetRequiredService<ILogger<SupplierPlatformService>>();
            ISupplierSelfTestHttpClient selfTestHttpClient = serviceProvider.GetRequiredService<IVodafoneSelfTestHttpClient>();
            var pollyRetryService = serviceProvider.GetRequiredService<IPollyRetryService>();

            var supplierApiClient = GetSupplierApiClient(supplier);
            var supplierConfig = GetSupplierConfiguration(supplier);

            return new SupplierPlatformService(logger, selfTestHttpClient, pollyRetryService, supplierApiClient, supplierConfig, supplier, featureToggles);
        }
        
        private VodafoneConfiguration GetSupplierConfiguration(Supplier supplier) =>
            supplier switch
            {
                Supplier.Vodafone => serviceProvider.GetRequiredService<IOptions<VodafoneConfiguration>>().Value,
                _ => throw new InvalidOperationException($"Unsupported supplier {supplier}")
            };
        
        private ISupplierApiClient GetSupplierApiClient(Supplier supplier) =>
            supplier switch
            {
                Supplier.Vodafone => serviceProvider.GetService<IVodafoneApiClient>(),
                _ => throw new InvalidOperationException($"Unsupported supplier {supplier}")
            };
    }
}
