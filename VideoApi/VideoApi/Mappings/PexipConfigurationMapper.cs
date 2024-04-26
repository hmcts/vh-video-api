using VideoApi.Common.Security.Supplier.Base;
using VideoApi.Contract.Responses;

namespace VideoApi.Mappings
{
    public static class PexipConfigurationMapper
    {
        public static PexipConfigResponse MapPexipConfigToResponse(SupplierConfiguration configuration)
        {   
            return new PexipConfigResponse
            {
                PexipSelfTestNode = configuration.PexipSelfTestNode
            };
        }
    }
}
