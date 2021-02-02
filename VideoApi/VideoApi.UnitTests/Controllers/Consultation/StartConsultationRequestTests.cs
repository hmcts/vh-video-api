using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services.Kinly;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Consultation
{
    public class StartConsultationRequestTests : ConsultationControllerTestBase
    {
        private Room _testRoom;

        [Test]
        public async Task Should_Return_Accepted()
        {
            var request = RequestBuilder();
            ConsultationService.Setup(x => x.GetAvailableConsultationRoomAsync(request.ConferenceId, request.RoomType))
                .ReturnsAsync(_testRoom);
            ConsultationService.Setup(x =>
                x.JoinConsultationRoomAsync(request.ConferenceId, request.RequestedBy, _testRoom.Label));

            var result = await Controller.StartConsultationRequestAsync(request);

            var actionResult = result.As<AcceptedResult>();
            actionResult.Should().NotBeNull();
        }

        [Test]
        public async Task Should_Throw_NotFoundException_When_Conference_Does_Not_Exist()
        {
            var request = RequestBuilder();
            request.ConferenceId = Guid.NewGuid();
            ConsultationService.Setup(x => x.GetAvailableConsultationRoomAsync(request.ConferenceId, request.RoomType))
                .ThrowsAsync(new ConferenceNotFoundException(request.ConferenceId));

            var result = await Controller.StartConsultationRequestAsync(request);

            var actionResult = result.As<NotFoundObjectResult>();
            actionResult.Should().NotBeNull();
        }

        [Test]
        public async Task Should_Return_NotFound_When_Participant_Cannot_Be_Found()
        {
            var request = RequestBuilder();
            request.RequestedBy = Guid.NewGuid();
            ConsultationService.Setup(x => x.GetAvailableConsultationRoomAsync(request.ConferenceId, request.RoomType))
                .ReturnsAsync(_testRoom);
            ConsultationService.Setup(x =>
                    x.JoinConsultationRoomAsync(request.ConferenceId, request.RequestedBy, _testRoom.Label))
                .ThrowsAsync(new ParticipantNotFoundException(request.RequestedBy));

            var result = await Controller.StartConsultationRequestAsync(request);

            var actionResult = result.As<NotFoundObjectResult>();
            actionResult.Should().NotBeNull();
        }

        [Test]
        public async Task Should_Return_BadRequest_When_Participant_Cannot_Be_Found()
        {
            var request = RequestBuilder();

            var kinlyApiException = new KinlyApiException("", (int) HttpStatusCode.BadRequest, "payload",
                new Dictionary<string, IEnumerable<string>>(), new Exception());

            ConsultationService.Setup(x => x.GetAvailableConsultationRoomAsync(request.ConferenceId, request.RoomType))
                .ReturnsAsync(_testRoom);
            ConsultationService
                .Setup(x => x.JoinConsultationRoomAsync(request.ConferenceId, request.RequestedBy, _testRoom.Label))
                .ThrowsAsync(kinlyApiException);

            var result = await Controller.StartConsultationRequestAsync(request);

            var actionResult = result.As<BadRequestObjectResult>();
            actionResult.Should().NotBeNull();
        }

        private StartConsultationRequest RequestBuilder()
        {
            if (TestConference.Participants == null)
            {
                Assert.Fail("No participants found in conference");
            }

            _testRoom = new Room(TestConference.Id, "JohRoom1", VirtualCourtRoomType.JudgeJOH);

            return new StartConsultationRequest
            {
                ConferenceId = TestConference.Id,
                RequestedBy = TestConference.GetParticipants().First(x =>
                    x.UserRole.Equals(UserRole.Judge)).Id,
                RoomType = VirtualCourtRoomType.JudgeJOH
            };
        }
    }
}
