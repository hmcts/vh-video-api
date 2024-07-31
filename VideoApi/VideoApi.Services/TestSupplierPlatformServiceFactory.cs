using System.Diagnostics.CodeAnalysis;
using VideoApi.Services.Contracts;
using Supplier = VideoApi.Domain.Enums.Supplier;

namespace VideoApi.Services
{
    [ExcludeFromCodeCoverage]
    public class TestSupplierPlatformServiceFactory(IVideoPlatformService supplierPlatformService) : ISupplierPlatformServiceFactory
    {
        public IVideoPlatformService Create(Supplier supplier) => supplierPlatformService;
    }
}
