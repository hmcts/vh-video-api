using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Extras.Moq;
using Moq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Extensions;
using VideoApi.Services;
using VideoApi.Services.Clients;
using VideoApi.Services.Clients.Models;
using VideoApi.Services.Contracts;
using RoomType = VideoApi.Domain.Enums.RoomType;
using Task = System.Threading.Tasks.Task;
using UserRole = VideoApi.Domain.Enums.UserRole;
using VirtualCourtRoomType = VideoApi.Domain.Enums.VirtualCourtRoomType;

namespace VideoApi.UnitTests.Services.Consultation
{
    [TestFixture]
    public class ConsultationServiceTests : ConsultationServiceTestBase
    {
        [SetUp]
        public void Setup()
        {
            _mocker = AutoMock.GetLoose();
            _vodafonePlatformService = new Mock<IVideoPlatformService>();
            _vodafonePlatformService.Setup(x => x.GetClient()).Returns(_mocker.Mock<ISupplierClient>().Object);
            _supplierPlatformServiceFactoryMock = _mocker.Mock<ISupplierPlatformServiceFactory>();
            _supplierPlatformServiceFactoryMock.Setup(x => x.Create(Supplier.Vodafone)).Returns(_vodafonePlatformService.Object);
            
            _consultationService = _mocker.Create<ConsultationService>();
            SetupTestConference();
            _request = InitConsultationRequestForJudge();
            _rooms = CreateTestRooms(_request);
        }
        
        private AutoMock _mocker;
        private ConsultationService _consultationService;
        private Mock<ISupplierPlatformServiceFactory> _supplierPlatformServiceFactoryMock;
        
        private StartConsultationRequest _request;
        private List<ConsultationRoom> _rooms;
        private Mock<IVideoPlatformService> _vodafonePlatformService;
        
        [Test]
        public async Task Should_Return_A_Valid_ConsultationRoom_With_A_Valid_Request()
        {
            _mocker.Mock<IQueryHandler>().Setup(x => x.Handle<GetAvailableConsultationRoomsByRoomTypeQuery, List<ConsultationRoom>>(
                It.Is<GetAvailableConsultationRoomsByRoomTypeQuery>(
                    query => query.ConferenceId.Equals(_request.ConferenceId) &&
                             query.CourtRoomType.Equals(_request.RoomType.MapToDomainEnum())))).ReturnsAsync(_rooms);

            var mockCommand = new CreateConsultationRoomCommand(_request.ConferenceId, "Judge", _request.RoomType.MapToDomainEnum(), false);
            _mocker.Mock<ICommandHandler>().Setup(x => x.Handle(mockCommand));

            _mocker.Mock<ISupplierClient>()
                .Setup(x => x.CreateConsultationRoomAsync(It.IsAny<Guid>(), It.IsAny<ConsultationRoomRequest>()))
                .ReturnsAsync(new CreateConsultationRoomResponse() {RoomLabel = "Label"});
            
            _mocker.Mock<IQueryHandler>().Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(TestConference);
            
            var returnedRoom =
                await _consultationService.GetAvailableConsultationRoomAsync(_request.ConferenceId, _request.RoomType.MapToDomainEnum());

            returnedRoom.Should().BeOfType<ConsultationRoom>();
            returnedRoom.Should().NotBeNull();
        }
        
        [Test]
        public async Task Should_close_empty_consultation_rooms_when_there_are_no_participants()
        {
            // Arrange
            foreach (var room in _rooms)
                room.RoomParticipants.Clear();
        
            _mocker.Mock<IQueryHandler>().Setup(x => x.Handle<GetAvailableConsultationRoomsByRoomTypeQuery, List<ConsultationRoom>>(
            It.Is<GetAvailableConsultationRoomsByRoomTypeQuery>(
                query => query.ConferenceId.Equals(_request.ConferenceId) &&
                         query.CourtRoomType.Equals(_request.RoomType.MapToDomainEnum())))).ReturnsAsync(_rooms);
        
            var mockCommand = new CreateConsultationRoomCommand(_request.ConferenceId, "Judge", _request.RoomType.MapToDomainEnum(), false);
            _mocker.Mock<ICommandHandler>().Setup(x => x.Handle(mockCommand));
                
            _mocker.Mock<ISupplierClient>()
                .Setup(x => x.CreateConsultationRoomAsync(It.IsAny<Guid>(), It.IsAny<ConsultationRoomRequest>()))
                .ReturnsAsync(new CreateConsultationRoomResponse() {RoomLabel = "Label"});
            
            _mocker.Mock<IQueryHandler>().Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(TestConference);
            
            // Act
            var returnedRoom =
                await _consultationService.GetAvailableConsultationRoomAsync(_request.ConferenceId, _request.RoomType.MapToDomainEnum());
        
            // Assert
            _mocker.Mock<ICommandHandler>().Verify(x => x.Handle(It.IsAny<CloseConsultationRoomCommand>()), Times.Exactly(_rooms.Count));
            _mocker.Mock<ISupplierClient>().Verify(x => x.CreateConsultationRoomAsync(It.Is<Guid>(y => y == _request.ConferenceId), It.Is<ConsultationRoomRequest>(
                y => y.RoomLabelPrefix.Equals(_request.RoomType.ToString()))), Times.Once);
            _mocker.Mock<ICommandHandler>().Verify(x => x.Handle(It.IsAny<CreateConsultationRoomCommand>()), Times.Once);
            returnedRoom.Should().BeOfType<ConsultationRoom>();
            returnedRoom.Should().NotBeNull();
        }
        
        [Test]
        public async Task Should_return_available_consultation_rooms()
        {
            // Arrange
            foreach (var room in _rooms)
                room.AddParticipant(new RoomParticipant(Guid.NewGuid()));

            _mocker.Mock<IQueryHandler>().Setup(x => x.Handle<GetAvailableConsultationRoomsByRoomTypeQuery, List<ConsultationRoom>>(
            It.Is<GetAvailableConsultationRoomsByRoomTypeQuery>(
                query => query.ConferenceId.Equals(_request.ConferenceId) &&
                         query.CourtRoomType.Equals(_request.RoomType.MapToDomainEnum())))).ReturnsAsync(_rooms);
            
            // Act
            var returnedRoom =
                await _consultationService.GetAvailableConsultationRoomAsync(_request.ConferenceId, _request.RoomType.MapToDomainEnum());
        
            // Assert
            _mocker.Mock<ICommandHandler>().Verify(x => x.Handle(It.IsAny<CloseConsultationRoomCommand>()), Times.Never);
            _mocker.Mock<ISupplierClient>().Verify(x => x.CreateConsultationRoomAsync(It.Is<Guid>(
                y => y == _request.ConferenceId), It.Is<ConsultationRoomRequest>(
                y => y.RoomLabelPrefix.Equals(_request.RoomType.ToString()))), Times.Never);
            _mocker.Mock<ICommandHandler>().Verify(x => x.Handle(It.IsAny<CreateConsultationRoomCommand>()), Times.Never);
            returnedRoom.Should().BeOfType<ConsultationRoom>();
            returnedRoom.Should().Be(_rooms[0]);
        }
        
        [Test]
        public async Task Should_Create_ConsultationRoom_If_None_Are_Available()
        {
            _mocker.Mock<IQueryHandler>()
                .Setup(x =>
                    x.Handle<GetAvailableConsultationRoomsByRoomTypeQuery, List<ConsultationRoom>>(It.IsAny<GetAvailableConsultationRoomsByRoomTypeQuery>()))
                .ReturnsAsync(new List<ConsultationRoom>());

            var consultationRoomParams = new ConsultationRoomRequest
            {
                RoomLabelPrefix = _request.RoomType.ToString()
            };

            _mocker.Mock<ISupplierClient>()
                .Setup(x => x.CreateConsultationRoomAsync(It.IsAny<Guid>(), It.IsAny<ConsultationRoomRequest>()))
                .ReturnsAsync(new CreateConsultationRoomResponse() {RoomLabel = "Label"});
            
            _mocker.Mock<IQueryHandler>().Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(TestConference);

            var returnedRoom =
                await _consultationService.GetAvailableConsultationRoomAsync(_request.ConferenceId, _request.RoomType.MapToDomainEnum());

            _mocker.Mock<ISupplierClient>().Verify(x => x.CreateConsultationRoomAsync(It.Is<Guid>(y => y == _request.ConferenceId), It.Is<ConsultationRoomRequest>(
                y => y.RoomLabelPrefix.Equals(consultationRoomParams.RoomLabelPrefix))), Times.Once);
            returnedRoom.Should().BeOfType<ConsultationRoom>();
            returnedRoom.Should().NotBeNull();
        }
        
        [Test]
        public async Task Should_Successfully_Transfer_Participant_To_ConsultationRoom()
        {
            _mocker.Mock<IQueryHandler>().Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(TestConference);

            var room = _rooms.First(x => x.ConferenceId.Equals(_request.ConferenceId));
            var participant =
                TestConference.Participants.First(x => x.Id.Equals(_request.RequestedBy));
            await _consultationService.ParticipantTransferToRoomAsync(_request.ConferenceId, _request.RequestedBy,
                room.Label);

            _vodafonePlatformService.Verify(x =>
                x.TransferParticipantAsync(TestConference.Id, 
                    participant.Id.ToString(), 
                    participant.GetCurrentRoom(), 
                    room.Label,
                    ConferenceRole.Host), Times.Exactly(1));

        }
        
        [Test]
        public async Task should_use_interpreter_room_when_participant_is_in_an_interpreter_room_on_transfer()
        {
            var roomId = 999;
            var participant =
                TestConference.Participants.First(x => x.Id.Equals(_request.RequestedBy));
            var interpreterRoom = new ParticipantRoom(TestConference.Id, "Interpreter1", VirtualCourtRoomType.Civilian);
            interpreterRoom.SetProtectedProperty(nameof(interpreterRoom.Id), roomId);
            var roomParticipant = new RoomParticipant(participant.Id)
            {
                Room = interpreterRoom,
                RoomId = interpreterRoom.Id
            };
            interpreterRoom.AddParticipant(roomParticipant);
            participant.RoomParticipants.Add(roomParticipant);
            
            _mocker.Mock<IQueryHandler>().Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(TestConference);

            var room = _rooms.First(x => x.ConferenceId.Equals(_request.ConferenceId));
            await _consultationService.ParticipantTransferToRoomAsync(_request.ConferenceId, _request.RequestedBy,
                room.Label);
            
            _vodafonePlatformService.Verify(x =>
                x.TransferParticipantAsync(TestConference.Id, roomId.ToString(), participant.GetCurrentRoom(), room.Label, ConferenceRole.Host), Times.Exactly(1));
        }
        
        private StartConsultationRequest InitConsultationRequestForJudge()
        {
            if (TestConference.Participants == null)
            {
                Assert.Fail("No participants found in conference");
            }

            return new StartConsultationRequest
            {
                ConferenceId = TestConference.Id,
                RequestedBy = TestConference.GetParticipants().First(x =>
                    x.UserRole.Equals(UserRole.Judge)).Id,
                RoomType = Contract.Enums.VirtualCourtRoomType.JudgeJOH
            };
        }
        
        private static List<ConsultationRoom> CreateTestRooms(StartConsultationRequest request)
        {
            var rooms = new List<ConsultationRoom>();
            var room1 = new ConsultationRoom(request.ConferenceId, "Judge", request.RoomType.MapToDomainEnum(), false);
            var room2 = new ConsultationRoom(Guid.NewGuid(), "Waiting", VirtualCourtRoomType.JudgeJOH, false);

            rooms.Add(room1);
            rooms.Add(room2);

            return rooms;
        }
        
        [Test]
        public async Task should_remove_a_participant_in_room()
        {
            var participant = TestConference.Participants[0];
            var consultationRoom = new ConsultationRoom(TestConference.Id, "ConsultationRoom2", VirtualCourtRoomType.Participant, false);
            consultationRoom.AddParticipant(new RoomParticipant(participant.Id));
            _mocker.Mock<IQueryHandler>().Setup(x => x.Handle<GetConsultationRoomByIdQuery, ConsultationRoom>(It.IsAny<GetConsultationRoomByIdQuery>())).ReturnsAsync(consultationRoom);
            var participantId = participant.Id;
            var fromRoom = consultationRoom.Label;
            var toRoom = "WaitingRoom";
            _mocker.Mock<IQueryHandler>().Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(TestConference);
            
            await _consultationService.LeaveConsultationAsync(TestConference.Id, participantId, fromRoom, toRoom);
            
            _vodafonePlatformService.Verify(x =>
                    x.TransferParticipantAsync(TestConference.Id, participantId.ToString(), fromRoom, toRoom, ConferenceRole.Host), Times.Exactly(1));
        }
        
        [Test]
        public async Task should_use_interpreter_room_when_participant_is_in_an_interpreter_room_on_leave()
        {
            var fromRoom = "ParticipantConsultationRoom1";
            var toRoom = RoomType.WaitingRoom.ToString();
            var roomId = 999;
            var participant =
                TestConference.Participants.First(x => x.Id.Equals(_request.RequestedBy));
            var interpreterRoom = new ParticipantRoom(TestConference.Id, "Interpreter1", VirtualCourtRoomType.Civilian);
            interpreterRoom.SetProtectedProperty(nameof(interpreterRoom.Id), roomId);
            var roomParticipant = new RoomParticipant(participant.Id)
            {
                Room = interpreterRoom,
                RoomId = interpreterRoom.Id
            };
            interpreterRoom.AddParticipant(roomParticipant);
            participant.RoomParticipants.Add(roomParticipant);
            
            _mocker.Mock<IQueryHandler>().Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(TestConference);

            await _consultationService.LeaveConsultationAsync(_request.ConferenceId, participant.Id, fromRoom, toRoom);
            
            _vodafonePlatformService.Verify(x =>
                x.TransferParticipantAsync(TestConference.Id, roomId.ToString(), fromRoom, toRoom, ConferenceRole.Host), Times.Exactly(1));
        }
        
        [Test]
        public async Task Should_Create_New_ConsultationRoom()
        {
            var mockCommand = new CreateConsultationRoomCommand(_request.ConferenceId, "Judge", _request.RoomType.MapToDomainEnum(), false);
            _mocker.Mock<ICommandHandler>().Setup(x => x.Handle(mockCommand));
            var consultationRoomParams = new ConsultationRoomRequest
            {
                RoomLabelPrefix = _request.RoomType.ToString()
            };

            _mocker.Mock<ISupplierClient>()
                .Setup(x => x.CreateConsultationRoomAsync(It.IsAny<Guid>(), It.IsAny<ConsultationRoomRequest>()))
                .ReturnsAsync(new CreateConsultationRoomResponse() { RoomLabel = "Label" });
            
            _mocker.Mock<IQueryHandler>().Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(TestConference);

            var returnedRoom =
                await _consultationService.CreateNewConsultationRoomAsync(_request.ConferenceId, _request.RoomType.MapToDomainEnum());

            _mocker.Mock<ISupplierClient>().Verify(x => x.CreateConsultationRoomAsync(It.Is<Guid>(y => y == _request.ConferenceId), It.Is<ConsultationRoomRequest>(
                y => y.RoomLabelPrefix.Equals(consultationRoomParams.RoomLabelPrefix))), Times.Once);
            returnedRoom.Should().BeOfType<ConsultationRoom>();
            returnedRoom.Should().NotBeNull();
            VerifySupplierUsed(TestConference.Supplier, Times.Once());
        }
        
        private void VerifySupplierUsed(VideoApi.Domain.Enums.Supplier supplier, Times times)
        {
            _supplierPlatformServiceFactoryMock.Verify(x => x.Create(supplier), times);
        }
    }
}
