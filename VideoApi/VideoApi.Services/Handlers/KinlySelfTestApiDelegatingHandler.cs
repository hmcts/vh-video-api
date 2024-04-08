using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using VideoApi.Common.Security.Supplier.Kinly;

namespace VideoApi.Services.Handlers
{
    public class KinlySelfTestApiDelegatingHandler(IKinlyJwtProvider tokenProvider) : DelegatingHandler
    {
        private const string ParticipantIdName = "participantId";

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var participantId = request.Properties.ContainsKey(ParticipantIdName) 
                ? request.Properties[ParticipantIdName] 
                : throw new Exception($"Could not find the field {ParticipantIdName} in the request properties dictionary");
            
            var token = tokenProvider.GenerateSelfTestApiToken(participantId.ToString(), 2);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            return base.SendAsync(request, cancellationToken);
        }
    }
}
