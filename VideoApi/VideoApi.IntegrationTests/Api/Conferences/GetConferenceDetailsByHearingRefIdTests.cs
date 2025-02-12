using System;
using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using Testing.Common.Helper;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.IntegrationTests.Api.Setup;
using VideoApi.IntegrationTests.Helper;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Api.Conferences;

public class GetConferenceDetailsByHearingRefIdTests : ApiTest
{
    [Test]
    public async Task should_get_conferences_details_by_HearingRefIds()
    {
        //arrange
        var vodafoneConference = await TestDataManager.SeedConference();
        using var client = Application.CreateClient();
        var payload = new GetConferencesByHearingIdsRequest
        {
            HearingRefIds =
            [
                vodafoneConference.HearingRefId
            ]
        };
        
        //act
        var result =
            await client.PostAsync(ApiUriFactory.ConferenceEndpoints.GetConferenceDetailsByHearingRefIds(),
                RequestBody.Set(payload));
        
        // assert
        result.IsSuccessStatusCode.Should().BeTrue(result.Content.ReadAsStringAsync().Result);
        var conferenceResponse = await ApiClientResponse.GetResponses<List<ConferenceDetailsResponse>>(result.Content);
        conferenceResponse.Count.Should().Be(1);
    }
    
    [TestCase("default guid")]
    [TestCase("null parameter")]
    [TestCase("empty list")]
    public async Task should_get_bad_request_when_querying_conferences_details_by_HearingRefIds(string parameter)
    {
        //arrange
        using var client = Application.CreateClient();
        GetConferencesByHearingIdsRequest payload = parameter switch
        {
            "default guid" => new GetConferencesByHearingIdsRequest { HearingRefIds = [Guid.Empty] },
            "null parameter" => new GetConferencesByHearingIdsRequest { HearingRefIds = null },
            "empty list" => new GetConferencesByHearingIdsRequest { HearingRefIds = [] },
            _ => throw new ArgumentOutOfRangeException(nameof(parameter), parameter, null)
        };
        
        //act
        var result =
            await client.PostAsync(ApiUriFactory.ConferenceEndpoints.GetConferenceDetailsByHearingRefIds(),
                RequestBody.Set(payload));
        
        // assert
        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Test]
    public async Task should_get_not_found_when_querying_conferences_details_by_HearingRefIds()
    {
        //arrange
        using var client = Application.CreateClient();
        var payload = new GetConferencesByHearingIdsRequest { HearingRefIds = [Guid.NewGuid()] };
        
        //act
        var result =
            await client.PostAsync(ApiUriFactory.ConferenceEndpoints.GetConferenceDetailsByHearingRefIds(),
                RequestBody.Set(payload));
        
        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
