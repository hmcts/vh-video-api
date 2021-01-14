using System;
using System.Collections.Generic;
using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services.Kinly;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Consultation
{
    public class LeaveConsultationTests : ConsultationControllerTestBase
    {
        [Test]
        public async Task should_return_not_found_when_no_matching_conference_is_found()
        {
            var leaveConsultationRequest = new LeaveConsultationRequest
                {ConferenceId = Guid.NewGuid(), ParticipantId = TestConference.Participants[0].Id};
            var result = await Controller.LeaveConsultationAsync(leaveConsultationRequest);
            var typedResult = (NotFoundResult) result;
            typedResult.Should().NotBeNull();
        }

        [Test]
        public async Task should_return_not_found_when_no_matching_participant_is_found()
        {
            var leaveConsultationRequest = new LeaveConsultationRequest
                {ConferenceId = TestConference.Id, ParticipantId = Guid.NewGuid()};
            var result = await Controller.LeaveConsultationAsync(leaveConsultationRequest);
            var typedResult = (NotFoundResult) result;
            typedResult.Should().NotBeNull();
        }

        [Test]
        public async Task should_return_judge_joh_to_waiting_room_for_valid_conference_and_room_type()
        {
            var conferenceId = TestConference.Id;
            var participantId = TestConference.Participants[0].Id;
            var vRoom = new Room(TestConference.Id, "ConsultationRoom", VirtualCourtRoomType.JudgeJOH);
            TestConference.Participants[0].CurrentVirtualRoom = vRoom;
            var fromRoom = "ConsultationRoom";
            var toRoom = "WaitingRoom";
            var leaveConsultationRequest = new LeaveConsultationRequest
                {ConferenceId = conferenceId, ParticipantId = participantId};
            await Controller.LeaveConsultationAsync(leaveConsultationRequest);

            QueryHandlerMock.Verify(q => q.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>
                (It.IsAny<GetConferenceByIdQuery>()), Times.Once);
            ConsultationService.Verify(v => v.LeaveConsultationAsync
                    (leaveConsultationRequest.ConferenceId, leaveConsultationRequest.ParticipantId, fromRoom, toRoom),
                Times.Once);
            VideoPlatformServiceMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Should_Return_BadRequest_When_Participant_Cannot_Be_Found()
        {
            var conferenceId = TestConference.Id;
            var participantId = TestConference.Participants[0].Id;
            var vRoom = new Room(TestConference.Id, "ConsultationRoom", VirtualCourtRoomType.JudgeJOH);
            TestConference.Participants[0].CurrentVirtualRoom = vRoom;
            var fromRoom = "ConsultationRoom";
            var toRoom = "WaitingRoom";
            var leaveConsultationRequest = new LeaveConsultationRequest
                {ConferenceId = conferenceId, ParticipantId = participantId};

            var kinlyApiException = new KinlyApiException("", (int) HttpStatusCode.BadRequest, "payload",
                new Dictionary<string, IEnumerable<string>>(), new Exception());

            ConsultationService.Setup(x => x.LeaveConsultationAsync(leaveConsultationRequest.ConferenceId,
                leaveConsultationRequest.ParticipantId, fromRoom, toRoom)).ThrowsAsync(kinlyApiException);

            var result = await Controller.LeaveConsultationAsync(leaveConsultationRequest);

            var actionResult = result.As<BadRequestObjectResult>();
            actionResult.Should().NotBeNull();
        }
    }
}
