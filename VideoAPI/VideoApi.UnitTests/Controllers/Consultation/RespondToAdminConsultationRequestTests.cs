using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using VideoApi.Contract.Requests;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Controllers.Consultation
{
    [TestFixture]
    public class RespondToAdminConsultationRequestTests : ConsultationControllerTestBase
    {
        [Test]
        public async Task should_transfer_participant_when_consultation_is_accepted()
        {
            var conferenceId = TestConference.Id;
            var participant = TestConference.GetParticipants()[3];

            var roomFrom = participant.CurrentRoom.Value;
            var request = new AdminConsultationRequest
            {
                ConferenceId = conferenceId,
                ParticipantId = participant.Id,
                ConsultationRoom = RoomType.ConsultationRoom1,
                Answer = ConsultationAnswer.Accepted
            };

            await Controller.RespondToAdminConsultationRequest(request);

            VideoPlatformServiceMock.Verify(x =>
                    x.TransferParticipantAsync(conferenceId, participant.Id, roomFrom, request.ConsultationRoom),
                Times.Once);
            VideoPlatformServiceMock.VerifyNoOtherCalls();
        }
        
        [Test]
        public async Task should_not_transfer_participant_when_consultation_is_not_accepted()
        {
            var conferenceId = TestConference.Id;
            var participant = TestConference.GetParticipants()[3];

            var roomFrom = participant.CurrentRoom.Value;
            var request = new AdminConsultationRequest
            {
                ConferenceId = conferenceId,
                ParticipantId = participant.Id,
                ConsultationRoom = RoomType.ConsultationRoom1,
                Answer = ConsultationAnswer.Rejected
            };

            await Controller.RespondToAdminConsultationRequest(request);

            VideoPlatformServiceMock.Verify(x =>
                    x.TransferParticipantAsync(conferenceId, participant.Id, roomFrom, request.ConsultationRoom),
                Times.Never);
            VideoPlatformServiceMock.VerifyNoOtherCalls();
        }
    }
}