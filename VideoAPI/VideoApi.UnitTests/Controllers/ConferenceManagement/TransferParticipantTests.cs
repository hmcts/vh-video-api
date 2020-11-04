using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using VideoApi.Contract.Requests;
using VideoApi.Domain.Enums;
using VideoApi.Services.Kinly;

namespace VideoApi.UnitTests.Controllers.ConferenceManagement
{
    public class TransferParticipantTests : ConferenceManagementControllerTestBase
    {
        [Test]
        public async Task should_move_participant_into_hearing_room()
        {
            var conferenceId = Guid.NewGuid();
            var request = new TransferParticipantRequest
            {
                ParticipantId = Guid.NewGuid(),
                TransferType = TransferType.Call
            };
            
            var result = await Controller.TransferParticipantAsync(conferenceId, request);
            result.Should().BeOfType<AcceptedResult>();
            
            VideoPlatformServiceMock.Verify(
                x => x.TransferParticipantAsync(conferenceId, request.ParticipantId, RoomType.WaitingRoom,
                    RoomType.HearingRoom), Times.Once);
        }
        
        [Test]
        public async Task should_dismiss_participant_from_hearing_room()
        {
            var conferenceId = Guid.NewGuid();
            var request = new TransferParticipantRequest
            {
                ParticipantId = Guid.NewGuid(),
                TransferType = TransferType.Dismiss
            };
            
            var result = await Controller.TransferParticipantAsync(conferenceId, request);
            result.Should().BeOfType<AcceptedResult>();

            VideoPlatformServiceMock.Verify(
                x => x.TransferParticipantAsync(conferenceId, request.ParticipantId, RoomType.HearingRoom,
                    RoomType.WaitingRoom), Times.Once);
        }
        
        [Test]
        public void should_return_internal_server_error_when_transfer_type_is_not_handled()
        {
            var conferenceId = Guid.NewGuid();
            var request = new TransferParticipantRequest
            {
                ParticipantId = Guid.NewGuid(),
                TransferType = null
            };

            Assert.ThrowsAsync<InvalidOperationException>(() =>
                Controller.TransferParticipantAsync(conferenceId, request));
            
            VideoPlatformServiceMock.Verify(
                x => x.TransferParticipantAsync(conferenceId, request.ParticipantId, It.IsAny<RoomType>(),
                    It.IsAny<RoomType>()), Times.Never);
        }

        [Test]
        public async Task should_return_kinly_status_code_on_error()
        {
            var conferenceId = Guid.NewGuid();
            var request = new TransferParticipantRequest
            {
                ParticipantId = Guid.NewGuid(),
                TransferType = TransferType.Call
            };
            var message = "Transfer Error";
            var response = "Unable to transfer participant";
            var statusCode = (int) HttpStatusCode.Unauthorized;
            var exception =
                new KinlyApiException(message, statusCode, response, null, null);
            VideoPlatformServiceMock
                .Setup(x => x.TransferParticipantAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<RoomType>(),
                    It.IsAny<RoomType>())).ThrowsAsync(exception);

            var result = await Controller.TransferParticipantAsync(conferenceId, request);
            result.Should().BeOfType<ObjectResult>();
            var typedResult = (ObjectResult) result;
            typedResult.StatusCode.Should().Be(statusCode);
            typedResult.Value.Should().Be(response);
        }
    }
}
