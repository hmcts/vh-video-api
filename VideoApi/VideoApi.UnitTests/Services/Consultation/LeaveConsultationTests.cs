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

        [SetUp]
        public void Setup()
        {
            _mocker = AutoMock.GetLoose();
            _sut = _mocker.Create<ConsultationService>();
        }

        [Test]
        public async Task should_transfer_linked_participant_out_of_consultation_when_last_non_linked_participant_leaves_consultation()
        {
            // arrange
            var conference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Applicant")
                .WithLinkedParticipant(UserRole.Individual, "Applicant")
                .WithInterpreterRoom()
                .Build();
            
            var interpreterRoom = conference.Rooms.OfType<ParticipantRoom>().First();
            var participant = conference.Participants.First(x => x is Participant && !((Participant)x).IsJudge() && x.GetParticipantRoom() == null);
            var consultationRoom = new ConsultationRoom(conference.Id, "ConsultationRoom2", VirtualCourtRoomType.Participant, false);

            foreach (var p in conference.Participants.Where(x=> x is Participant && !((Participant)x).IsJudge()))
            {
                consultationRoom.AddParticipant(new RoomParticipant(p.Id));
            }
            
            _mocker.Mock<IQueryHandler>().Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(
                    It.Is<GetConferenceByIdQuery>(q => q.ConferenceId == conference.Id)))
                .ReturnsAsync(conference);
            
            _mocker.Mock<IQueryHandler>().Setup(x => x.Handle<GetConsultationRoomByIdQuery, ConsultationRoom>(It.IsAny<GetConsultationRoomByIdQuery>())).ReturnsAsync(consultationRoom);

            // act
            await _sut.LeaveConsultationAsync(conference.Id, participant.Id, consultationRoom.Label,
                RoomType.WaitingRoom.ToString());

            // assert
            _mocker.Mock<IKinlyApiClient>().Verify(x =>
                    x.TransferParticipantAsync(conference.Id.ToString(),
                        It.Is<TransferParticipantParams>(r =>
                            r.From == consultationRoom.Label &&
                            r.To == RoomType.WaitingRoom.ToString() &&
                            r.Part_id == participant.Id.ToString())
                    )
                , Times.Once);
            
            _mocker.Mock<IKinlyApiClient>().Verify(x =>
                    x.TransferParticipantAsync(conference.Id.ToString(),
                        It.Is<TransferParticipantParams>(r =>
                            r.From == consultationRoom.Label &&
                            r.To == RoomType.WaitingRoom.ToString() &&
                            r.Part_id == interpreterRoom.Id.ToString())
                    )
                , Times.Once);
        }

        [Test]
        public async Task should_not_transfer_linked_participant_out_of_consultation_when_non_linked_participants_are_still_in_consultation()
        {
            // arrange
            var conference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Applicant")
                .WithLinkedParticipant(UserRole.Individual, "Applicant")
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithInterpreterRoom()
                .Build();
            
            var interpreterRoom = conference.Rooms.OfType<ParticipantRoom>().First();
            var participant = conference.Participants.First(x => x is Participant && !((Participant)x).IsJudge() && x.GetParticipantRoom() == null);
            var consultationRoom = new ConsultationRoom(conference.Id, "ConsultationRoom2", VirtualCourtRoomType.Participant, false);

            foreach (var p in conference.Participants.Where(x=> x is Participant && !((Participant)x).IsJudge()))
            {
                consultationRoom.AddParticipant(new RoomParticipant(p.Id));
            }
            
            _mocker.Mock<IQueryHandler>().Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(
                    It.Is<GetConferenceByIdQuery>(q => q.ConferenceId == conference.Id)))
                .ReturnsAsync(conference);
            
            _mocker.Mock<IQueryHandler>().Setup(x => x.Handle<GetConsultationRoomByIdQuery, ConsultationRoom>(It.IsAny<GetConsultationRoomByIdQuery>())).ReturnsAsync(consultationRoom);

            // act
            await _sut.LeaveConsultationAsync(conference.Id, participant.Id, consultationRoom.Label,
                RoomType.WaitingRoom.ToString());

            // assert
            _mocker.Mock<IKinlyApiClient>().Verify(x =>
                    x.TransferParticipantAsync(conference.Id.ToString(),
                        It.Is<TransferParticipantParams>(r =>
                            r.From == consultationRoom.Label &&
                            r.To == RoomType.WaitingRoom.ToString() &&
                            r.Part_id == participant.Id.ToString())
                    )
                , Times.Once);
            
            _mocker.Mock<IKinlyApiClient>().Verify(x =>
                    x.TransferParticipantAsync(conference.Id.ToString(),
                        It.Is<TransferParticipantParams>(r =>
                            r.From == consultationRoom.Label &&
                            r.To == RoomType.WaitingRoom.ToString() &&
                            r.Part_id == interpreterRoom.Id.ToString())
                    )
                , Times.Never);
        }
        
        [Test]
        public async Task should_not_transfer_participant_out_of_consultation_when_non_linked_participants_are_still_in_consultation()
        {
            // arrange
            var conference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Applicant")
                .WithParticipant(UserRole.Representative, "Applicant")
                .WithParticipant(UserRole.Representative, "Respondent")
                .Build();
            
            var participant = conference.Participants.First(x => x is Participant && !((Participant)x).IsJudge() && x.GetParticipantRoom() == null);
            var consultationRoom = new ConsultationRoom(conference.Id, "ConsultationRoom2", VirtualCourtRoomType.Participant, false);

            foreach (var p in conference.Participants.Where(x=> x is Participant && !((Participant)x).IsJudge()))
            {
                consultationRoom.AddParticipant(new RoomParticipant(p.Id));
            }
            
            _mocker.Mock<IQueryHandler>().Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(
                    It.Is<GetConferenceByIdQuery>(q => q.ConferenceId == conference.Id)))
                .ReturnsAsync(conference);
            
            _mocker.Mock<IQueryHandler>().Setup(x => x.Handle<GetConsultationRoomByIdQuery, ConsultationRoom>(It.IsAny<GetConsultationRoomByIdQuery>())).ReturnsAsync(consultationRoom);

            // act
            await _sut.LeaveConsultationAsync(conference.Id, participant.Id, consultationRoom.Label,
                RoomType.WaitingRoom.ToString());

            // assert
            _mocker.Mock<IKinlyApiClient>().Verify(x =>
                    x.TransferParticipantAsync(conference.Id.ToString(),
                        It.Is<TransferParticipantParams>(r =>
                            r.From == consultationRoom.Label &&
                            r.To == RoomType.WaitingRoom.ToString() &&
                            r.Part_id == participant.Id.ToString())
                    )
                , Times.Once);
        }
    }
}
