using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using VideoApi.Contract.Requests;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Controllers.Consultation
{
    [TestFixture]
    public class RespondToConsultationRequestResponseTests : ConsultationControllerTestBase
    {
        [Test]
        public async Task Should_transfer_participant_when_consultation_is_accepted()
        {
            var conferenceId = TestConference.Id;
            var participant = TestConference.GetParticipants()[3];

            var roomFrom = participant.GetCurrentRoom();
            var request = new ConsultationRequestResponse
            {
                ConferenceId = conferenceId,
                RequestedFor = participant.Id,
                RoomLabel = RoomType.ConsultationRoom.ToString(),
                Answer = ConsultationAnswer.Accepted
            };

            await Controller.RespondToConsultationRequestAsync(request);

            VideoPlatformServiceMock.Verify(x =>
                    x.TransferParticipantAsync(conferenceId, participant.Id, roomFrom, request.RoomLabel),
                Times.Once);
            VideoPlatformServiceMock.VerifyNoOtherCalls();
        }
        
        [Test]
        public async Task Should_not_transfer_participant_when_consultation_is_not_accepted()
        {
            var conferenceId = TestConference.Id;
            var participant = TestConference.GetParticipants()[3];

            var roomFrom = participant.GetCurrentRoom();
            var request = new ConsultationRequestResponse
            {
                ConferenceId = conferenceId,
                RequestedFor = participant.Id,
                RoomLabel = RoomType.ConsultationRoom.ToString(),
                Answer = ConsultationAnswer.Rejected
            };

            await Controller.RespondToConsultationRequestAsync(request);

            VideoPlatformServiceMock.Verify(x =>
                    x.TransferParticipantAsync(conferenceId, participant.Id, roomFrom, request.RoomLabel),
                Times.Never);
            VideoPlatformServiceMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Should_return_notfound_when_no_matching_participant_is_found()
        {
            var conferenceId = TestConference.Id;
             
            var request = new ConsultationRequestResponse
            {
                ConferenceId = conferenceId,
                RequestedFor = Guid.NewGuid(),
                RoomLabel = RoomType.ConsultationRoom.ToString(),
                Answer = ConsultationAnswer.Rejected
            };

            var result = await Controller.RespondToConsultationRequestAsync(request);
            var typedResult = (NotFoundResult)result;
            typedResult.Should().NotBeNull();
        }

        [Test]
        public async Task should_return_bad_request_when_answer_is_not_provided()
        {
            var conferenceId = TestConference.Id;
            var request = new ConsultationRequestResponse
            {
                ConferenceId = conferenceId,
                RequestedFor = Guid.NewGuid(),
                RoomLabel = RoomType.ConsultationRoom.ToString()
            };
            
            var result = await Controller.RespondToConsultationRequestAsync(request);
            var typedResult = (BadRequestObjectResult)result;
            typedResult.Should().NotBeNull();
        }
    }
}
