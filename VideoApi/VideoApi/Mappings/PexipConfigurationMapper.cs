using VideoApi.Common.Security.Kinly;
using VideoApi.Contract.Responses;

namespace VideoApi.Mappings
{
    public static class PexipConfigurationMapper
    {
        public static PexipConfigResponse MapPexipConfigToResponse(KinlyConfiguration kinlyConfiguration)
        {   
            return new PexipConfigResponse
            {
                PexipSelfTestNode = kinlyConfiguration.PexipSelfTestNode
            };
        }
    }
}
