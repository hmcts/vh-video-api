using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services.Clients;
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
            var consultationRoom = new ConsultationRoom(TestConference.Id, "ConsultationRoom", VirtualCourtRoomType.JudgeJOH, false);
            TestConference.Participants[0].CurrentConsultationRoom = consultationRoom;
            TestConference.Participants[0].CurrentConsultationRoomId = 1;
            QueryHandlerMock
                .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(
                    It.Is<GetConferenceByIdQuery>(q => q.ConferenceId == TestConference.Id)))
                .ReturnsAsync(TestConference);

            var fromRoom = "ConsultationRoom";
            var toRoom = "WaitingRoom";
            var leaveConsultationRequest = new LeaveConsultationRequest
                {ConferenceId = conferenceId, ParticipantId = participantId};
            await Controller.LeaveConsultationAsync(leaveConsultationRequest);

            QueryHandlerMock.Verify(q => q.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>
                (It.IsAny<GetConferenceByIdQuery>()), Times.Once);
            ConsultationServiceMock.Verify(v => v.LeaveConsultationAsync
                    (leaveConsultationRequest.ConferenceId, leaveConsultationRequest.ParticipantId, fromRoom, toRoom),
                Times.Once);
        }

        [Test]
        public async Task Should_Return_BadRequest_When_Participant_Cannot_Be_Found_In_Consultation_Room()
        {
            var conferenceId = TestConference.Id;
            var participantId = TestConference.Participants[0].Id;
            var consultationRoom = new ConsultationRoom(TestConference.Id, "ConsultationRoom", VirtualCourtRoomType.JudgeJOH, false);
            TestConference.Participants[0].CurrentConsultationRoom = consultationRoom;
            var fromRoom = "ConsultationRoom";
            var toRoom = "WaitingRoom";
            var leaveConsultationRequest = new LeaveConsultationRequest
                {ConferenceId = conferenceId, ParticipantId = participantId};

            var kinlyApiException = new SupplierApiException("", (int) HttpStatusCode.BadRequest, "payload",
                new Dictionary<string, IEnumerable<string>>(), new Exception());

            ConsultationServiceMock.Setup(x => x.LeaveConsultationAsync(leaveConsultationRequest.ConferenceId,
                leaveConsultationRequest.ParticipantId, fromRoom, toRoom)).ThrowsAsync(kinlyApiException);

            var result = await Controller.LeaveConsultationAsync(leaveConsultationRequest);

            QueryHandlerMock.Verify(q => q.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>
                (It.IsAny<GetConferenceByIdQuery>()), Times.Once);
            ConsultationServiceMock.Verify(v => v.LeaveConsultationAsync
                    (leaveConsultationRequest.ConferenceId, leaveConsultationRequest.ParticipantId, fromRoom, toRoom),
                Times.Never);
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
