using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
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
            var participantsSummary = (List<ParticipantSummaryResponse>)typedResult.Value;
            participantsSummary.Count.Should().Be(5);

            var participant = participantsSummary[1];
            var exspectedParticipant = TestConference.Participants[1];

            participant.CaseGroup.Should().Be(exspectedParticipant.CaseTypeGroup);
            participant.Id.Should().Be(exspectedParticipant.Id);
            participant.Username.Should().Be(exspectedParticipant.Username);
            participant.DisplayName.Should().Be(exspectedParticipant.DisplayName);
            participant.FirstName.Should().Be(exspectedParticipant.FirstName);
            participant.LastName.Should().Be(exspectedParticipant.LastName);
            participant.Status.Should().Be(exspectedParticipant.State);
            participant.UserRole.Should().Be(exspectedParticipant.UserRole);
            participant.HearingRole.Should().Be(exspectedParticipant.HearingRole);
            participant.Representee.Should().Be(exspectedParticipant.Representee);
            participant.ContactEmail.Should().Be(exspectedParticipant.ContactEmail);
            participant.ContactTelephone.Should().Be(exspectedParticipant.ContactTelephone);
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
