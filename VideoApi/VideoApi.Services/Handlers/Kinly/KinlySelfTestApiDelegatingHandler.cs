using VideoApi.Common.Security.Supplier.Kinly;
using VideoApi.Services.Handlers.Base;

namespace VideoApi.Services.Handlers.Kinly;

public class KinlySelfTestApiDelegatingHandler(IKinlyJwtProvider tokenProvider)
    : CustomSelfTestApiDelegatingHandler(tokenProvider);

