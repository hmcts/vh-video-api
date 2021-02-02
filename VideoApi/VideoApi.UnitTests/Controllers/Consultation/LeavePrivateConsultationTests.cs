using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Queries;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Consultation
{
    public class LeavePrivateConsultationTests : ConsultationControllerTestBase
    {
        [Test]
        public async Task Should_leave_private_consultation_with_valid_conference_and_room_type()
        {
            var conferenceId = TestConference.Id;
            var request = TestConference.GetParticipants()[1];

            var leaveConsultationRequest = new LeaveConsultationRequest { ConferenceId = conferenceId, 
                ParticipantId = request.Id  };

            await Controller.LeavePrivateConsultationAsync(leaveConsultationRequest);

            QueryHandlerMock.Verify(q => q.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()), Times.Once);
            VideoPlatformServiceMock.Verify(v => v.StopPrivateConsultationAsync(TestConference, RoomType.ConsultationRoom1), Times.Once);
            VideoPlatformServiceMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Should_return_notfound_when_no_matching_participant_is_found()
        {
            var conferenceId = TestConference.Id;

            var leaveConsultationRequest = new LeaveConsultationRequest
            {
                ConferenceId = conferenceId,
                ParticipantId = Guid.NewGuid()
            };

            var result = await Controller.LeavePrivateConsultationAsync(leaveConsultationRequest);
            var typedResult = (NotFoundResult)result;
            typedResult.Should().NotBeNull();
        }

        [Test]
        public async Task Should_return_notfound_when_no_matching_conference_is_found()
        {
            var leaveConsultationRequest = new LeaveConsultationRequest
            {
                ConferenceId = Guid.NewGuid(),
                ParticipantId = Guid.NewGuid()
            };

            var result = await Controller.LeavePrivateConsultationAsync(leaveConsultationRequest);
            var typedResult = (NotFoundResult)result;
            typedResult.Should().NotBeNull();
        }

        [Test]
        public async Task Should_return_nocontent_when_participant_current_room_is_not_consultation_room_type()
        {
            var conferenceId = TestConference.Id;
            var request = TestConference.GetParticipants()[2];

            var leaveConsultationRequest = new LeaveConsultationRequest
            {
                ConferenceId = conferenceId,
                ParticipantId = request.Id
            };

            var result = await Controller.LeavePrivateConsultationAsync(leaveConsultationRequest);
            var typedResult = (NoContentResult)result;
            typedResult.Should().NotBeNull();
        }
    }
}