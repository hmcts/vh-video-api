using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.IntegrationTests.Api.Setup;
using VideoApi.IntegrationTests.Helper;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Api.Conferences;

public class StartVideoHearingTests : ApiTest
{
    [Test]
    public async Task start_hearing_should_have_success_response()
    {
        //assert
        var conference = await TestDataManager.SeedConference();
        using var client = Application.CreateClient();
        var host = conference.Participants.Where(x => x.UserRole == Domain.Enums.UserRole.Judge).Select(x => x.Id.ToString());
        var payload = RequestHelper.Serialise(new StartHearingRequest { ParticipantsToForceTransfer = host });
       
        //act
        var result =
            await client.PostAsync(ApiUriFactory.ConferenceManagementEndpoints.StartVideoHearing(conference.Id), 
                    new StringContent(payload, Encoding.UTF8,"application/json"));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();

    }
}
