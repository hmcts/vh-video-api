using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Testing.Common.Helper;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.IntegrationTests.Api.Setup;
using VideoApi.IntegrationTests.Helper;

namespace VideoApi.IntegrationTests.Api.Participants
{
    public class UpdateParticipantUsernameTests : ApiTest
    {
        [Test]
        public async Task should_update_participant_username()
        {
            // Arrange
            var seededConference = await TestDataManager.SeedConference();
            var seededParticipant = seededConference.Participants[0];
            var newUsername = seededParticipant.Username + "_updated";
            var request = new UpdateParticipantUsernameRequest
            {
                Username = newUsername
            };

            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.ParticipantsEndpoints.UpdateParticipantUsername(seededParticipant.Id), RequestBody.Set(request));

            var getConference = await client.GetAsync(ApiUriFactory.ParticipantsEndpoints.GetParticipantsByConferenceId(seededConference.Id));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var participantsResponse = await ApiClientResponse.GetResponses<List<ParticipantResponse>>(getConference.Content);
            var participant = participantsResponse.First(x => x.Id == seededParticipant.Id);
            
            participant.Username.Should().Be(newUsername);
        }

        [Test]
        public async Task should_return_not_found_when_participant_does_not_exist()
        {
            // Arrange
            const string newUsername = "participant_name_updated";
            var request = new UpdateParticipantUsernameRequest
            {
                Username = newUsername
            };

            // Act
            using var client = Application.CreateClient();
            var result = await client
                .PatchAsync(ApiUriFactory.ParticipantsEndpoints.UpdateParticipantUsername(Guid.NewGuid()), RequestBody.Set(request));

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
