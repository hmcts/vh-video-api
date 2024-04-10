using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VideoApi.Common.Security.Supplier.Base;

namespace VideoApi.Services.Handlers.Base;

public abstract class CustomApiTokenDelegatingHandler : DelegatingHandler
{
    private readonly ICustomJwtTokenProvider _tokenProvider;

    protected CustomApiTokenDelegatingHandler(ICustomJwtTokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _tokenProvider.GenerateApiToken("hmcts-video-api-client", 2);
        request.Headers.Add("Authorization", $"Bearer {token}");
        return base.SendAsync(request, cancellationToken);
    }
}
