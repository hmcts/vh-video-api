using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using VideoApi.Common.Security.Supplier.Base;
namespace VideoApi.Services.Handlers.Base;

public abstract class CustomSelfTestApiDelegatingHandler : DelegatingHandler
{
    private readonly ICustomJwtTokenProvider _tokenProvider;

    protected CustomSelfTestApiDelegatingHandler(ICustomJwtTokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    private const string ParticipantIdName = "participantId";

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
#pragma warning disable CS0618 // For compatibility with the supplier
        var participantId = request.Properties.TryGetValue(ParticipantIdName, out var property) 
            ? property 
            : throw new KeyNotFoundException($"Could not find the field {ParticipantIdName} in the request properties dictionary");
#pragma warning restore CS0618 // For compatibility with the supplier
            
        var token = _tokenProvider.GenerateSelfTestApiToken(participantId.ToString(), 2);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
        return base.SendAsync(request, cancellationToken);
    }
}
