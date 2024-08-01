using System.Collections.Generic;
using NUnit.Framework;
using Testing.Common.Helper;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.Domain.Enums;
using VideoApi.IntegrationTests.Api.Setup;
using VideoApi.IntegrationTests.Helper;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Api.Conferences;

public class GetConferencesForAdminByHearingRefIdTests : ApiTest
{
    [Test]
    public async Task should_get_conferences_for_admin_by_HearingRefId()
    {
        //arrange
        var kinlyConference = await TestDataManager.SeedConference(Supplier.Kinly);
        var vodafoneConference = await TestDataManager.SeedConference();
        using var client = Application.CreateClient();
        var payload = new GetConferencesByHearingIdsRequest 
        { 
            HearingRefIds =
            [
                kinlyConference.HearingRefId, 
                vodafoneConference.HearingRefId
            ]
        };
       
        //act
        var result =
            await client.PostAsync(ApiUriFactory.ConferenceEndpoints.GetConferencesForAdminByHearingRefId(), RequestBody.Set(payload));
       
        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        var conferenceResponse = await ApiClientResponse.GetResponses<List<ConferenceForAdminResponse>>(result.Content);
        conferenceResponse.Count.Should().Be(2);
        
        var kinlyConferenceResponse = conferenceResponse.Find(x => x.HearingRefId == kinlyConference.HearingRefId);
        VerifyConferenceInResponse(kinlyConferenceResponse, kinlyConference);
        
        var vodafoneConfigurationResponse = conferenceResponse.Find(x => x.HearingRefId == vodafoneConference.HearingRefId);
        VerifyConferenceInResponse(vodafoneConfigurationResponse, vodafoneConference);
    }
}
