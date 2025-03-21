using System;
using System.Diagnostics.CodeAnalysis;
using VideoApi.Common.Security.Supplier.Base;
using VideoApi.Common.Security.Supplier.Vodafone;
using VideoApi.Services.Contracts;
using Supplier = VideoApi.Domain.Enums.Supplier;

namespace VideoApi.Services
{
    [ExcludeFromCodeCoverage]
    public class TestSupplierPlatformServiceFactory(VodafoneConfiguration vodafoneConfiguration) : ISupplierPlatformServiceFactory
    {
        public IVideoPlatformService Create(Supplier supplier) =>
            supplier switch
            {
                Supplier.Vodafone => new SupplierPlatformServiceStub(vodafoneConfiguration),
                _ => throw new ArgumentOutOfRangeException(nameof(supplier), supplier, null)
            };

        public SupplierConfiguration GetSupplierConfiguration(Supplier supplier) => vodafoneConfiguration;
    }
}
