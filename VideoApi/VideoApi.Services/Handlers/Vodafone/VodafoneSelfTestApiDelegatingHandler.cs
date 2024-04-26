using VideoApi.Common.Security.Supplier.Vodafone;
using VideoApi.Services.Handlers.Base;

namespace VideoApi.Services.Handlers.Vodafone;

public class VodafoneSelfTestApiDelegatingHandler : CustomSelfTestApiDelegatingHandler
{
    public VodafoneSelfTestApiDelegatingHandler(IVodafoneJwtProvider tokenProvider) : base(tokenProvider){}
}

