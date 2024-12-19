using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using VideoApi.Domain;
using VideoApi.Extensions;
using Task = System.Threading.Tasks.Task;

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

        private void AssertRoomIsCreatedLocked(StartConsultationRequest request)
        {
            ConsultationServiceMock.Verify(x => x.CreateNewConsultationRoomAsync(request.ConferenceId,
                It.IsAny<VideoApi.Domain.Enums.VirtualCourtRoomType>(), true), Times.Once);
        }
    }
}
