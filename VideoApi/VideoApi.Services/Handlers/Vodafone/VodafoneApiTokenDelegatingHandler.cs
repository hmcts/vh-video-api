using VideoApi.Common.Security.Supplier.Vodafone;
using VideoApi.Services.Handlers.Base;

namespace VideoApi.Services.Handlers.Vodafone;

public class VodafoneApiTokenDelegatingHandler : CustomApiTokenDelegatingHandler
{
    public VodafoneApiTokenDelegatingHandler(IVodafoneJwtProvider tokenProvider) : base(tokenProvider){}
}
