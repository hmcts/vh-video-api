using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Queries;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Participant
{
    public class GetParticipantsByConferenceIdTest : ParticipantsControllerTestBase
    {
        [Test]
        public async Task Should_return_ok_result_for_given_conference_id_and_participants()
        {
            MockQueryHandler
                .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(TestConference);

            var result = await Controller.GetParticipantsByConferenceId(TestConference.Id);

            var typedResult = (OkObjectResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var participantsSummary = (List<ParticipantResponse>)typedResult.Value;
            participantsSummary.Count.Should().Be(5);

            var participant = participantsSummary[1];
            var expectedParticipant = TestConference.Participants[1];

            participant.Id.Should().Be(expectedParticipant.Id);
            participant.Username.Should().Be(expectedParticipant.Username);
            participant.DisplayName.Should().Be(expectedParticipant.DisplayName);
            participant.CurrentStatus.Should().Be((ParticipantState)expectedParticipant.State);
            participant.UserRole.Should().Be((UserRole)expectedParticipant.UserRole);
        }

        [Test]
        public async Task Should_return_notfound_with_no_matching_conference()
        {
            MockQueryHandler
                .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync((VideoApi.Domain.Conference)null);


            var result = await Controller.GetParticipantsByConferenceId(Guid.NewGuid());
            var typedResult = (NotFoundResult)result;
            typedResult.Should().NotBeNull();
        }

    }
}
