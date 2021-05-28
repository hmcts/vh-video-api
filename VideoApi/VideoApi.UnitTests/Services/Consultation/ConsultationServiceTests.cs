using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Extras.Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
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
using VideoApi.Services.Kinly;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Services.Consultation
{
    [TestFixture]
    public class ConsultationServiceTests : ConsultationServiceTestBase
    {
        private AutoMock _mocker;
        private ConsultationService _consultationService;


        private StartConsultationRequest _request;
        private List<ConsultationRoom> _rooms;

        [SetUp]
        public void Setup()
        {
            _mocker = AutoMock.GetLoose();
            _consultationService = _mocker.Create<ConsultationService>();

            SetupTestConference();
            _request = InitConsultationRequestForJudge();
            _rooms = CreateTestRooms(_request);
        }

        [Test]
        public async Task Should_Return_A_Valid_ConsultationRoom_With_A_Valid_Request()
        {
            _mocker.Mock<IQueryHandler>().Setup(x => x.Handle<GetAvailableConsultationRoomsByRoomTypeQuery, List<ConsultationRoom>>(
                It.Is<GetAvailableConsultationRoomsByRoomTypeQuery>(
                    query => query.ConferenceId.Equals(_request.ConferenceId) &&
                             query.CourtRoomType.Equals(_request.RoomType.MapToDomainEnum())))).ReturnsAsync(_rooms);

            var mockCommand = new CreateConsultationRoomCommand(_request.ConferenceId, "Judge", _request.RoomType.MapToDomainEnum(), false);
            _mocker.Mock<ICommandHandler>().Setup(x => x.Handle(mockCommand));

            _mocker.Mock<IKinlyApiClient>()
                .Setup(x => x.CreateConsultationRoomAsync(It.IsAny<string>(), It.IsAny<CreateConsultationRoomParams>()))
                .ReturnsAsync(new CreateConsultationRoomResponse() {Room_label = "Label"});
            
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
                
            _mocker.Mock<IKinlyApiClient>()
                .Setup(x => x.CreateConsultationRoomAsync(It.IsAny<string>(), It.IsAny<CreateConsultationRoomParams>()))
                .ReturnsAsync(new CreateConsultationRoomResponse() {Room_label = "Label"});
            
            // Act
            var returnedRoom =
                await _consultationService.GetAvailableConsultationRoomAsync(_request.ConferenceId, _request.RoomType.MapToDomainEnum());
        
            // Assert
            _mocker.Mock<ICommandHandler>().Verify(x => x.Handle(It.IsAny<CloseConsultationRoomCommand>()), Times.Exactly(_rooms.Count));
            _mocker.Mock<IKinlyApiClient>().Verify(x => x.CreateConsultationRoomAsync(It.Is<string>(
                y => y.Equals(_request.ConferenceId.ToString())), It.Is<CreateConsultationRoomParams>(
                y => y.Room_label_prefix.Equals(_request.RoomType.ToString()))), Times.Once);
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
            _mocker.Mock<IKinlyApiClient>().Verify(x => x.CreateConsultationRoomAsync(It.Is<string>(
                y => y.Equals(_request.ConferenceId.ToString())), It.Is<CreateConsultationRoomParams>(
                y => y.Room_label_prefix.Equals(_request.RoomType.ToString()))), Times.Never);
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

            var consultationRoomParams = new CreateConsultationRoomParams
            {
                Room_label_prefix = _request.RoomType.ToString()
            };

            _mocker.Mock<IKinlyApiClient>()
                .Setup(x => x.CreateConsultationRoomAsync(It.IsAny<string>(), It.IsAny<CreateConsultationRoomParams>()))
                .ReturnsAsync(new CreateConsultationRoomResponse() {Room_label = "Label"});

            var returnedRoom =
                await _consultationService.GetAvailableConsultationRoomAsync(_request.ConferenceId, _request.RoomType.MapToDomainEnum());

            _mocker.Mock<IKinlyApiClient>().Verify(x => x.CreateConsultationRoomAsync(It.Is<string>(
                y => y.Equals(_request.ConferenceId.ToString())), It.Is<CreateConsultationRoomParams>(
                y => y.Room_label_prefix.Equals(consultationRoomParams.Room_label_prefix))), Times.Once);
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

            var request = new TransferParticipantParams
            {
                From = participant.GetCurrentRoom(),
                To = room.Label,
                Part_id = participant.Id.ToString()
            };

            _mocker.Mock<IKinlyApiClient>().Verify(x => x.TransferParticipantAsync(It.Is<string>(
                    y => y.Equals(_request.ConferenceId.ToString())), It.Is<TransferParticipantParams>(
                    y => y.From.Equals(request.From) && y.To.Equals(request.To) && y.Part_id.Equals(request.Part_id))),
                Times.Once);
        }

        [Test]
        public async Task should_use_interpreter_room_when_participant_is_in_an_interpreter_room_on_transfer()
        {
            var participant =
                TestConference.Participants.First(x => x.Id.Equals(_request.RequestedBy));
            var interpreterRoom = new ParticipantRoom(TestConference.Id, "Interpreter1", VirtualCourtRoomType.Civilian);
            interpreterRoom.SetProtectedProperty(nameof(interpreterRoom.Id), 999);
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
            
            
            var request = new TransferParticipantParams
            {
                From = participant.GetCurrentRoom(),
                To = room.Label,
                Part_id = interpreterRoom.Id.ToString()
            };
            
            _mocker.Mock<IKinlyApiClient>().Verify(x => x.TransferParticipantAsync(It.Is<string>(
                    y => y.Equals(_request.ConferenceId.ToString())), It.Is<TransferParticipantParams>(
                    y => y.From.Equals(request.From) && y.To.Equals(request.To) && y.Part_id.Equals(request.Part_id))),
                Times.Once);
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

        private List<ConsultationRoom> CreateTestRooms(StartConsultationRequest request)
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

            _mocker.Mock<IKinlyApiClient>().Verify(x =>
                    x.TransferParticipantAsync(TestConference.Id.ToString(),
                        It.Is<TransferParticipantParams>(r =>
                            r.From == fromRoom &&
                            r.To == toRoom
                        )
                    )
                , Times.Exactly(1));
        }
        
        [Test]
        public async Task should_use_interpreter_room_when_participant_is_in_an_interpreter_room_on_leave()
        {
            var fromRoom = "ParticipantConsultationRoom1";
            var toRoom = RoomType.WaitingRoom.ToString();
            var participant =
                TestConference.Participants.First(x => x.Id.Equals(_request.RequestedBy));
            var interpreterRoom = new ParticipantRoom(TestConference.Id, "Interpreter1", VirtualCourtRoomType.Civilian);
            interpreterRoom.SetProtectedProperty(nameof(interpreterRoom.Id), 999);
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
            
            
            var request = new TransferParticipantParams
            {
                From = fromRoom,
                To = toRoom,
                Part_id = interpreterRoom.Id.ToString()
            };
            
            _mocker.Mock<IKinlyApiClient>().Verify(x => x.TransferParticipantAsync(It.Is<string>(
                    y => y.Equals(_request.ConferenceId.ToString())), It.Is<TransferParticipantParams>(
                    y => y.From.Equals(request.From) && y.To.Equals(request.To) && y.Part_id.Equals(request.Part_id))),
                Times.Once);
        }

        [Test]
        public async Task Should_Create_New_ConsultationRoom()
        {
            var mockCommand = new CreateConsultationRoomCommand(_request.ConferenceId, "Judge", _request.RoomType.MapToDomainEnum(), false);
            _mocker.Mock<ICommandHandler>().Setup(x => x.Handle(mockCommand));
            var consultationRoomParams = new CreateConsultationRoomParams
            {
                Room_label_prefix = _request.RoomType.ToString()
            };

            _mocker.Mock<IKinlyApiClient>()
                .Setup(x => x.CreateConsultationRoomAsync(It.IsAny<string>(), It.IsAny<CreateConsultationRoomParams>()))
                .ReturnsAsync(new CreateConsultationRoomResponse() { Room_label = "Label" });

            var returnedRoom =
                await _consultationService.CreateNewConsultationRoomAsync(_request.ConferenceId, _request.RoomType.MapToDomainEnum());

            _mocker.Mock<IKinlyApiClient>().Verify(x => x.CreateConsultationRoomAsync(It.Is<string>(
                y => y.Equals(_request.ConferenceId.ToString())), It.Is<CreateConsultationRoomParams>(
                y => y.Room_label_prefix.Equals(consultationRoomParams.Room_label_prefix))), Times.Once);
            returnedRoom.Should().BeOfType<ConsultationRoom>();
            returnedRoom.Should().NotBeNull();
        }
    }
}
