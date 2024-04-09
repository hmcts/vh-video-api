using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VideoApi.Common.Security.Supplier.Vodafone;

namespace VideoApi.Services.Handlers
{
    public class VodafoneApiTokenDelegatingHandler : DelegatingHandler
    {
        private readonly IVodafoneJwtProvider _tokenProvider;

        public VodafoneApiTokenDelegatingHandler(IVodafoneJwtProvider tokenProvider)
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
}
