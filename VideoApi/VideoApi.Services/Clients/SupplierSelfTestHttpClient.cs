using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VideoApi.Common.Helpers;
using VideoApi.Common.Security.Supplier.Base;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.Services.Clients;

public class SupplierSelfTestHttpClient(
    HttpClient httpClient,
    SupplierConfiguration supplierConfigOptions,
    ILogger<SupplierSelfTestHttpClient> logger)
    : IKinlySelfTestHttpClient, IVodafoneSelfTestHttpClient
{
    public async Task<TestCallResult> GetTestCallScoreAsync(Guid participantId)
    {
        logger.LogInformation("Retrieving test call score for participant {ParticipantId} at {SelfTestApiUrl}", participantId, supplierConfigOptions.SelfTestApiUrl);
        
        var requestUri = $"{supplierConfigOptions.SelfTestApiUrl}/testcall/{participantId}";
#pragma warning disable CS0618 // For compatibility with the supplier
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(requestUri),
            Method = HttpMethod.Get,
            Properties = {{"participantId", participantId}}
        };
#pragma warning restore CS0618 // For compatibility with the supplier
        
        var responseMessage = await httpClient.SendAsync(request);
        
        if (responseMessage.StatusCode == HttpStatusCode.NotFound)
        {
            logger.LogWarning(" {StatusCode} : Failed to retrieve self test score for participant {ParticipantId}", responseMessage.StatusCode, participantId);
            return null;
        }
        
        var content = await responseMessage.Content.ReadAsStringAsync();
        var testCall = ApiRequestHelper.Deserialise<Testcall>(content);
        logger.LogWarning(" {StatusCode} : Successfully retrieved self test score for participant {ParticipantId}", responseMessage.StatusCode, participantId);
        
        return new TestCallResult(testCall.Passed, (TestScore) testCall.Score);
    }
}
