using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoApi.Common.Configuration;
using VideoApi.Contract.Responses;

namespace Video.API.Mappings
{
    public class PexipConfigurationMapper
    {
        public PexipConfigResponse MapPexipConfigToResponse(ServicesConfiguration serviceConfiguration)
        {
            if (serviceConfiguration == null) return null;
            return new PexipConfigResponse
            {
                PexipSelfTestNode = serviceConfiguration.PexipSelfTestNode
            };
        }
    }
}
