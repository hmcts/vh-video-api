using System.Collections.Generic;
using VideoApi.Common.Security.Supplier.Base;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.Services.Mappers
{
    public class SupplierConfigurationMapper(ISupplierPlatformServiceFactory supplierPlatformServiceFactory)
    {
        public List<SupplierConfigurationMapping> ExtractSupplierConfigurations(List<Conference> conferences)
        {
            var supplierConfigurations = new List<SupplierConfigurationMapping>();
            
            CheckAndAddConfiguration(Supplier.Vodafone);

            return supplierConfigurations;

            void CheckAndAddConfiguration(Supplier supplier)
            {
                if (!conferences.Exists(x => x.Supplier == supplier))
                {
                    return;
                }
                
                var platformService = supplierPlatformServiceFactory.Create(supplier);
                var configuration = platformService.GetSupplierConfiguration();
                supplierConfigurations.Add(new SupplierConfigurationMapping(supplier, configuration));
            }
        }
    }
    
    public class SupplierConfigurationMapping(Supplier supplier, SupplierConfiguration configuration)
    {
        public Supplier Supplier { get; private set; } = supplier;
        public SupplierConfiguration Configuration { get; private set; } = configuration;
    }
}
