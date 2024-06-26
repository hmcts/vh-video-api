using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.IntegrationTests.Api.Setup;
using VideoApi.IntegrationTests.Helper;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Api.Conferences;

public class GetConferencesForAdminByHearingRefIdTests : ApiTest
{
    [Test]
    public async Task should_get_conferences_for_admin_by_HearingRefId()
    {
        //assert
        var conference = await TestDataManager.SeedConference();
        using var client = Application.CreateClient();
        var payload = new GetConferencesByHearingIdsRequest { HearingRefIds = new[] { conference.HearingRefId } };
       
        //act
        var result =
            await client.PostAsync(ApiUriFactory.ConferenceEndpoints.GetConferencesForAdminByHearingRefId(), RequestBody.Set(payload));
       
        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        var conferenceResponse = await ApiClientResponse.GetResponses<List<ConferenceForAdminResponse>>(result.Content);
        var resultConference = conferenceResponse.FirstOrDefault();
        resultConference.Should().NotBeNull();
        resultConference?.Id.Should().Be(conference.Id);
    }
}
