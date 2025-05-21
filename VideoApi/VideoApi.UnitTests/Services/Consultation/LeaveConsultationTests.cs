using System.Linq;
using Autofac.Extras.Moq;
using Moq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Services;
using VideoApi.Services.Clients;
using VideoApi.Services.Contracts;
using ConferenceRole = VideoApi.Domain.Enums.ConferenceRole;
using RoomType = VideoApi.Domain.Enums.RoomType;
using Task = System.Threading.Tasks.Task;
using UserRole = VideoApi.Domain.Enums.UserRole;
using VirtualCourtRoomType = VideoApi.Domain.Enums.VirtualCourtRoomType;

namespace VideoApi.UnitTests.Services.Consultation
{
    public class LeaveConsultationTests
    {
        private AutoMock _mocker;
        private Mock<IVideoPlatformService> _supplierPlatformService;
        private ConsultationService _sut;
        
        [SetUp]
        public void Setup()
        {
            _mocker = AutoMock.GetLoose();
            _supplierPlatformService = _mocker.Mock<IVideoPlatformService>();
            _supplierPlatformService.Setup(x => x.GetClient()).Returns(_mocker.Mock<ISupplierClient>().Object);
            var supplierPlatformServiceFactory = _mocker.Mock<ISupplierPlatformServiceFactory>();
            supplierPlatformServiceFactory.Setup(x => x.Create(VideoApi.Domain.Enums.Supplier.Vodafone)).Returns(_supplierPlatformService.Object);
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
            
            _mocker.Mock<IQueryHandler>().Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(conference);

            // act
            await _sut.LeaveConsultationAsync(conference.Id, participant.Id, consultationRoom.Label,
                RoomType.WaitingRoom.ToString());

            // assert
            _supplierPlatformService.Verify(x =>
                x.TransferParticipantAsync(conference.Id,
                    interpreterRoom.Id.ToString(),
                    consultationRoom.Label,
                    RoomType.WaitingRoom.ToString(),
                    It.IsAny<ConferenceRole>()), Times.Exactly(1));
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
            
            _mocker.Mock<IQueryHandler>().Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(conference);

            // act
            await _sut.LeaveConsultationAsync(conference.Id, participant.Id, consultationRoom.Label, RoomType.WaitingRoom.ToString());

            // assert
            _supplierPlatformService.Verify(x =>
                x.TransferParticipantAsync(conference.Id,
                    participant.Id.ToString(),
                    consultationRoom.Label,
                    RoomType.WaitingRoom.ToString(),
                    It.IsAny<ConferenceRole>()), Times.Exactly(1));
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
            
            _mocker.Mock<IQueryHandler>().Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(conference);

            // act
            await _sut.LeaveConsultationAsync(conference.Id, participant.Id, consultationRoom.Label, RoomType.WaitingRoom.ToString());

            // assert
            _supplierPlatformService.Verify(x =>
                x.TransferParticipantAsync(conference.Id,
                    participant.Id.ToString(), 
                    consultationRoom.Label, 
                    RoomType.WaitingRoom.ToString(), 
                    It.IsAny<ConferenceRole>()), Times.Exactly(1));
        }
    }
}
