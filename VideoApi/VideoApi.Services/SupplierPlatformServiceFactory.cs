using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VideoApi.Common.Security.Supplier.Base;
using VideoApi.Common.Security.Supplier.Stub;
using VideoApi.Common.Security.Supplier.Vodafone;
using VideoApi.Services.Clients;
using VideoApi.Services.Clients.SupplierStub;
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
            var logger = serviceProvider.GetRequiredService<ILogger<SupplierPlatformService>>();
            var pollyRetryService = serviceProvider.GetRequiredService<IPollyRetryService>();

            var supplierApiClient = GetSupplierApiClient(supplier);
            var supplierConfig = GetSupplierConfiguration(supplier);

            return new SupplierPlatformService(logger, pollyRetryService, supplierApiClient, supplierConfig, supplier);
        }
        
        private SupplierConfiguration GetSupplierConfiguration(Supplier supplier) =>
            supplier switch
            {
                Supplier.Vodafone => serviceProvider.GetRequiredService<IOptions<VodafoneConfiguration>>().Value,
                Supplier.Stub => serviceProvider.GetRequiredService<IOptions<SupplierStubConfiguration>>().Value,
                _ => throw new InvalidOperationException($"Unsupported supplier {supplier}")
            };
        
        private ISupplierApiClient GetSupplierApiClient(Supplier supplier) =>
            supplier switch
            {
                Supplier.Vodafone => serviceProvider.GetService<IVodafoneApiClient>(),
                Supplier.Stub => serviceProvider.GetService<ISupplierStubApiClient>(),
                _ => throw new InvalidOperationException($"Unsupported supplier {supplier}")
            };
    }
}
