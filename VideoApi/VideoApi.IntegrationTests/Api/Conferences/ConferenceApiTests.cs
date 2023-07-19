using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using SpecFlow.Internal.Json;
using Testing.Common.Helper;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.IntegrationTests.Api.Setup;
using VideoApi.IntegrationTests.Helper;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Api.Conferences;

public class ConferenceApiTests : ApiTest
{
    [Test]
    public async Task should_get_conferences_for_admin_by_HearingRefId()
    {
        //assert
        var conference = await TestDataManager.SeedConference();
        using var client = Application.CreateClient();
        var payload = RequestHelper.Serialise(new GetConferencesByHearingIdsRequest { HearingRefIds = new[] { conference.HearingRefId  } });
       
        //act
        var result =
            await client.PostAsync(ApiUriFactory.ConferenceEndpoints.GetConferencesForAdminByHearingRefId(), 
                    new StringContent(payload, Encoding.UTF8,"application/json"));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        var conferenceResponse = await ApiClientResponse.GetResponses<List<ConferenceForAdminResponse>>(result.Content);
        var resultConference = conferenceResponse.FirstOrDefault();
        resultConference.Should().NotBeNull();
        resultConference?.Id.Should().Be(conference.Id);
    }
    
    [Test]
    public async Task should_get_conferences_for_host_by_HearingRefId()
    {
        //arrange
        var conference = await TestDataManager.SeedConference();
        using var client = Application.CreateClient();    
        var payload = RequestHelper.Serialise(new GetConferencesByHearingIdsRequest { HearingRefIds = new[] { conference.HearingRefId  } });

       
        //act
        var result =
            await client.PostAsync(ApiUriFactory.ConferenceEndpoints.GetConferencesForHostByHearingRefId(), 
                new StringContent(payload, Encoding.UTF8,"application/json"));


        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        var conferenceResponse = await ApiClientResponse.GetResponses<List<ConferenceForHostResponse>>(result.Content);
        var resultConference = conferenceResponse.FirstOrDefault();
        resultConference.Should().NotBeNull();
        resultConference?.Id.Should().Be(conference.Id);
    }
}
