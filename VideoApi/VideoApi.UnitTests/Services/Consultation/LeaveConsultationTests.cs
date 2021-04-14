using System.Linq;
using Autofac.Extras.Moq;
using Moq;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services;
using VideoApi.Services.Kinly;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Services.Consultation
{
    public class LeaveConsultationTests
    {
        private AutoMock _mocker;
        private ConsultationService _sut;
        private Conference _conference;

        [SetUp]
        public void Setup()
        {
            _mocker = AutoMock.GetLoose();
            _sut = _mocker.Create<ConsultationService>();
            
            _conference = _conference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Applicant")
                .WithLinkedParticipant(UserRole.Individual, "Applicant")
                .WithInterpreterRoom()
                .Build();
        }

        [Test]
        public async Task should_transfer_linked_participant_out_of_consultation_when_last_non_linked_participant_leaves_consultation()
        {
            // arrange
            var interpreterRoom = _conference.Rooms.OfType<ParticipantRoom>().First();
            var participant = _conference.Participants.First(x => !x.IsJudge() && x.GetParticipantRoom() == null);
            var consultationRoom = new ConsultationRoom(_conference.Id, "ConsultationRoom2", VirtualCourtRoomType.Participant, false);

            foreach (var p in _conference.Participants.Where(x=> !x.IsJudge()))
            {
                consultationRoom.AddParticipant(new RoomParticipant(p.Id));
            }
            
            _mocker.Mock<IQueryHandler>().Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(
                    It.Is<GetConferenceByIdQuery>(q => q.ConferenceId == _conference.Id)))
                .ReturnsAsync(_conference);
            
            _mocker.Mock<IQueryHandler>().Setup(x => x.Handle<GetConsultationRoomByIdQuery, ConsultationRoom>(It.IsAny<GetConsultationRoomByIdQuery>())).ReturnsAsync(consultationRoom);

            // act
            await _sut.LeaveConsultationAsync(_conference.Id, participant.Id, consultationRoom.Label,
                RoomType.WaitingRoom.ToString());

            // assert
            _mocker.Mock<IKinlyApiClient>().Verify(x =>
                    x.TransferParticipantAsync(_conference.Id.ToString(),
                        It.Is<TransferParticipantParams>(r =>
                            r.From == consultationRoom.Label &&
                            r.To == RoomType.WaitingRoom.ToString() &&
                            r.Part_id == participant.Id.ToString())
                    )
                , Times.Once);
            
            _mocker.Mock<IKinlyApiClient>().Verify(x =>
                    x.TransferParticipantAsync(_conference.Id.ToString(),
                        It.Is<TransferParticipantParams>(r =>
                            r.From == consultationRoom.Label &&
                            r.To == RoomType.WaitingRoom.ToString() &&
                            r.Part_id == interpreterRoom.Id.ToString())
                    )
                , Times.Once);
        }
    }
}
