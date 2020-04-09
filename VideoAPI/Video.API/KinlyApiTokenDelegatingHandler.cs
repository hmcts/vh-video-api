using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VideoApi.Common.Security.Kinly;

namespace Video.API
{
    public class KinlyApiTokenDelegatingHandler : DelegatingHandler
    {
        private readonly ICustomJwtTokenProvider _tokenProvider;

        public KinlyApiTokenDelegatingHandler(ICustomJwtTokenProvider tokenProvider)
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