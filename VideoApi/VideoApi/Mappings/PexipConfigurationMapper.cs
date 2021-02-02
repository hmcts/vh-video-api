using VideoApi.Common.Configuration;
using VideoApi.Contract.Responses;

namespace VideoApi.Mappings
{
    public static class PexipConfigurationMapper
    {
        public static PexipConfigResponse MapPexipConfigToResponse(ServicesConfiguration serviceConfiguration)
        {   
            return new PexipConfigResponse
            {
                PexipSelfTestNode = serviceConfiguration.PexipSelfTestNode
            };
        }
    }
}
