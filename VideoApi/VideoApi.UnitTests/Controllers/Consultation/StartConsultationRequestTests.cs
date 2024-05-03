using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using VideoApi.Extensions;
using VideoApi.Services.Clients;
using Task = System.Threading.Tasks.Task;
using VhRoom = VideoApi.Domain.Enums.RoomType;

namespace VideoApi.UnitTests.Controllers.Consultation
{
    public class StartConsultationRequestTests : ConsultationControllerTestBase
    {
        private ConsultationRoom _testConsultationRoom;

        [Test]
        public async Task Should_Return_Ok()
        {
            var request = RequestBuilder();
            ConsultationServiceMock.Setup(x => x.GetAvailableConsultationRoomAsync(request.ConferenceId, request.RoomType.MapToDomainEnum()))
                .ReturnsAsync(_testConsultationRoom);
            ConsultationServiceMock.Setup(x =>
                x.ParticipantTransferToRoomAsync(request.ConferenceId, request.RequestedBy, _testConsultationRoom.Label));

            var result = await Controller.StartConsultationRequestAsync(request);

            result.Should().BeOfType<AcceptedResult>();
        }

        [Test]
        public async Task Should_Throw_NotFoundException_When_Conference_Does_Not_Exist()
        {
            var request = RequestBuilder();
            request.ConferenceId = Guid.NewGuid();
            ConsultationServiceMock.Setup(x => x.GetAvailableConsultationRoomAsync(request.ConferenceId, request.RoomType.MapToDomainEnum()))
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
            ConsultationServiceMock.Setup(x => x.GetAvailableConsultationRoomAsync(request.ConferenceId, request.RoomType.MapToDomainEnum()))
                .ReturnsAsync(_testConsultationRoom);
            ConsultationServiceMock.Setup(x =>
                    x.ParticipantTransferToRoomAsync(request.ConferenceId, request.RequestedBy, _testConsultationRoom.Label))
                .ThrowsAsync(new ParticipantNotFoundException(request.RequestedBy));

            var result = await Controller.StartConsultationRequestAsync(request);

            var actionResult = result.As<NotFoundObjectResult>();
            actionResult.Should().NotBeNull();
        }

        [Test]
        public async Task Should_Return_BadRequest_When_Participant_Cannot_Be_Found()
        {
            var request = RequestBuilder();

            var kinlyApiException = new SupplierApiException("", (int) HttpStatusCode.BadRequest, "payload",
                new Dictionary<string, IEnumerable<string>>(), new Exception());

            ConsultationServiceMock.Setup(x => x.GetAvailableConsultationRoomAsync(request.ConferenceId, request.RoomType.MapToDomainEnum()))
                .ReturnsAsync(_testConsultationRoom);
            ConsultationServiceMock
                .Setup(x => x.ParticipantTransferToRoomAsync(request.ConferenceId, request.RequestedBy, _testConsultationRoom.Label))
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

            _testConsultationRoom = new ConsultationRoom(TestConference.Id, "JohRoom1", VirtualCourtRoomType.JudgeJOH.MapToDomainEnum(), false);

            return new StartConsultationRequest
            {
                ConferenceId = TestConference.Id,
                RequestedBy = TestConference.GetParticipants().First(x =>
                    x.UserRole.MapToContractEnum().Equals(UserRole.Judge)).Id,
                RoomType = VirtualCourtRoomType.JudgeJOH
            };
        }

        [Test]
        public async Task Should_Start_New_Consultation_Returns_Ok()
        {
            var request = RequestBuilder();
            ConsultationServiceMock.Setup(x => x.CreateNewConsultationRoomAsync(request.ConferenceId,
                It.IsAny<VideoApi.Domain.Enums.VirtualCourtRoomType>(), It.IsAny<bool>())).ReturnsAsync(_testConsultationRoom);

            ConsultationServiceMock.Setup(x =>
                x.ParticipantTransferToRoomAsync(request.ConferenceId, request.RequestedBy, _testConsultationRoom.Label));

            var result = await Controller.StartNewConsultationRequestAsync(request);

            result.Should().BeOfType<OkObjectResult>();
            AssertRoomIsCreatedLocked(request);
        }

        [Test]
        public async Task Should_Start_New_Consultation_Returns_Notfound_conference()
        {
            var request = RequestBuilder();
            ConsultationServiceMock.Setup(x => x.CreateNewConsultationRoomAsync(request.ConferenceId,
                It.IsAny<VideoApi.Domain.Enums.VirtualCourtRoomType>(), It.IsAny<bool>()))
                .ThrowsAsync(new ConferenceNotFoundException(request.ConferenceId));

            ConsultationServiceMock.Setup(x =>
                x.ParticipantTransferToRoomAsync(request.ConferenceId, request.RequestedBy, _testConsultationRoom.Label));

            var result = await Controller.StartNewConsultationRequestAsync(request);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public async Task Should_Start_New_Consultation_Returns_Notfound_participant()
        {
            var request = RequestBuilder();
            ConsultationServiceMock.Setup(x => x.CreateNewConsultationRoomAsync(request.ConferenceId,
                It.IsAny<VideoApi.Domain.Enums.VirtualCourtRoomType>(), It.IsAny<bool>()))
                .ThrowsAsync(new ParticipantNotFoundException(Guid.NewGuid()));

            ConsultationServiceMock.Setup(x =>
                x.ParticipantTransferToRoomAsync(request.ConferenceId, request.RequestedBy, _testConsultationRoom.Label));

            var result = await Controller.StartNewConsultationRequestAsync(request);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public async Task Should_Start_New_Consultation_Returns_BadRequest_for_kinly_exeption()
        {
            var request = RequestBuilder();
            ConsultationServiceMock.Setup(x => x.CreateNewConsultationRoomAsync(request.ConferenceId,
                It.IsAny<VideoApi.Domain.Enums.VirtualCourtRoomType>(), It.IsAny<bool>()))
                .ThrowsAsync(new SupplierApiException("Error", 400, "Response", null, null));

            ConsultationServiceMock.Setup(x =>
                x.ParticipantTransferToRoomAsync(request.ConferenceId, request.RequestedBy, _testConsultationRoom.Label));

            var result = await Controller.StartNewConsultationRequestAsync(request);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        private void AssertRoomIsCreatedLocked(StartConsultationRequest request)
        {
            ConsultationServiceMock.Verify(x => x.CreateNewConsultationRoomAsync(request.ConferenceId,
                It.IsAny<VideoApi.Domain.Enums.VirtualCourtRoomType>(), true), Times.Once);
        }
    }
}
