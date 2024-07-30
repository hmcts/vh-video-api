using System.Diagnostics.CodeAnalysis;
using VideoApi.Contract.Enums;
using VideoApi.Services.Contracts;

namespace VideoApi.Services
{
    [ExcludeFromCodeCoverage]
    public class TestSupplierPlatformServiceFactory(IVideoPlatformService supplierPlatformService) : ISupplierPlatformServiceFactory
    {
        public IVideoPlatformService Create(Supplier supplier) => supplierPlatformService;
    }
}
