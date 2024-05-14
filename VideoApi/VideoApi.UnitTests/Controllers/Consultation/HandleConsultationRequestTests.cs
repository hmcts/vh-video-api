using System;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Consultation
{
    public class HandleConsultationRequestTests : ConsultationControllerTestBase
    {
        [Test]
        public async Task Should_raise_notification_to_requester_and_admin_when_consultation_is_accepted()
        {
            var conferenceId = TestConference.Id;
            var requestedBy = TestConference.GetParticipants()[2];
            var requestedFor = TestConference.GetParticipants()[3];
            
            var answer = ConsultationAnswer.Accepted;

            var request = new ConsultationRequestResponse
            {
                ConferenceId = conferenceId,
                RequestedBy = requestedBy.Id,
                RequestedFor = requestedFor.Id,
                Answer = answer,
                RoomLabel = "Room1"
            };

            await Controller.RespondToConsultationRequestAsync(request);

            CommandHandlerMock.Verify(x => x.Handle(It.Is<SaveEventCommand>(s => s.Reason == $"Adding {requestedFor.DisplayName} to {request.RoomLabel}")), Times.Once);
            ConsultationServiceMock.Verify(x =>
                x.ParticipantTransferToRoomAsync(TestConference.Id, requestedFor.Id, "Room1"), Times.Once);
            ConsultationServiceMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Should_return_notfound_when_no_requested_by_participant_is_found()
        {
            var conferenceId = TestConference.Id;
            var requestedBy = TestConference.GetParticipants()[2];

            var answer = ConsultationAnswer.Accepted;

            var request = new ConsultationRequestResponse
            {
                ConferenceId = conferenceId,
                RequestedBy = requestedBy.Id,
                RequestedFor = Guid.NewGuid(),
                Answer = answer
            };

            var result = await Controller.RespondToConsultationRequestAsync(request);
            var typedResult = (NotFoundResult)result;
            typedResult.Should().NotBeNull();
        }

        [Test]
        public async Task Should_return_notfound_when_no_requested_for_participant_is_found()
        {
            var conferenceId = TestConference.Id;
            var requestedFor = TestConference.GetParticipants()[3];

            var answer = ConsultationAnswer.Accepted;

            var request = new ConsultationRequestResponse
            {
                ConferenceId = conferenceId,
                RequestedBy = Guid.NewGuid(),
                RequestedFor = requestedFor.Id,
                Answer = answer
            };

            var result = await Controller.RespondToConsultationRequestAsync(request);
            var typedResult = (NotFoundResult)result;
            typedResult.Should().NotBeNull();
        }

        [Test]
        public async Task Should_return_ok_for_lock_room_request()
        {
            var conferenceId = TestConference.Id;

            var request = new LockRoomRequest
            {
                ConferenceId = conferenceId,
                RoomLabel = "ConsultationRoom",
                Lock = true
            };

            var result = await Controller.LockRoomRequestAsync(request);
            var typedResult = (OkResult)result;
            typedResult.Should().NotBeNull();
        }

        [Test]
        public async Task Should_return_notfound_for_lock_room_request()
        {
            var conferenceId = TestConference.Id;

            var request = new LockRoomRequest
            {
                ConferenceId = conferenceId,
                RoomLabel = "ConsultationRoom",
                Lock = true
            };
            CommandHandlerMock
               .Setup(x => x.Handle(It.IsAny<LockRoomCommand>())).Throws(new RoomNotFoundException(12345));

            var result = await Controller.LockRoomRequestAsync(request);
            var typedResult = (NotFoundObjectResult)result;
            typedResult.Should().NotBeNull();
        }
    }
}
