using VideoApi.Common.Configuration;
using VideoApi.Contract.Responses;

namespace Video.API.Mappings
{
    public static class PexipConfigurationMapper
    {
        public static PexipConfigResponse MapPexipConfigToResponse(ServicesConfiguration serviceConfiguration)
        {
            if (serviceConfiguration == null)
            {
                return null;
            }
            
            return new PexipConfigResponse
            {
                PexipSelfTestNode = serviceConfiguration.PexipSelfTestNode
            };
        }
    }
}
