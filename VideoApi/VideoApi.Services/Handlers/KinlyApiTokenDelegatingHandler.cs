using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VideoApi.Common.Security.Supplier.Kinly;

namespace VideoApi.Services.Handlers
{
    public class KinlyApiTokenDelegatingHandler : DelegatingHandler
    {
        private readonly IKinlyJwtProvider _tokenProvider;

        public KinlyApiTokenDelegatingHandler(IKinlyJwtProvider tokenProvider)
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
