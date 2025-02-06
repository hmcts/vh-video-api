using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.Services.Clients;

public class SupplierSelfTestHttpClient(
    ISupplierApiClient supplierApiClient,
    ILogger<SupplierSelfTestHttpClient> logger)
    : IVodafoneSelfTestHttpClient
{
    public async Task<TestCallResult> GetTestCallScoreAsync(Guid participantId)
    {
        logger.LogInformation("Retrieving test call score for participant {ParticipantId}", participantId);

        try
        {
            var testCall = await supplierApiClient.RetrieveParticipantSelfTestScore(participantId);
            return new TestCallResult(testCall.Passed, (TestScore) testCall.Score);
        }
        catch (SupplierApiException e)
        {
            if (e.StatusCode != HttpStatusCode.NotFound) throw;
            logger.LogWarning(e, "Failed to retrieve self test score for participant {ParticipantId}", participantId);
            return null;

        }
    }
}
