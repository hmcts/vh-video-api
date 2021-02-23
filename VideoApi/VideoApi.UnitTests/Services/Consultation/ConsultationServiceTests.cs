using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
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
        private ConsultationService _consultationService;
        private Mock<IKinlyApiClient> _kinlyApiClient;
        private Mock<ILogger<ConsultationService>> _logger;
        private Mock<ICommandHandler> _commandHandler;
        private Mock<IQueryHandler> _queryHandler;

        private StartConsultationRequest _request;
        private List<Room> _rooms;

        [SetUp]
        public void Setup()
        {
            _kinlyApiClient = new Mock<IKinlyApiClient>();
            _logger = new Mock<ILogger<ConsultationService>>();
            _commandHandler = new Mock<ICommandHandler>();
            _queryHandler = new Mock<IQueryHandler>();
            _consultationService = new ConsultationService(_kinlyApiClient.Object, _logger.Object, _commandHandler.Object, _queryHandler.Object);

            SetupTestConference();
            _request = RequestBuilder();
            _rooms = CreateTestRooms(_request);
        }

        [Test]
        public async Task Should_Return_A_Valid_ConsultationRoom_With_A_Valid_Request()
        {
            _queryHandler.Setup(x => x.Handle<GetAvailableRoomByRoomTypeQuery, List<Room>>(
                It.Is<GetAvailableRoomByRoomTypeQuery>(
                    query => query.ConferenceId.Equals(_request.ConferenceId) &&
                             query.CourtRoomType.Equals(_request.RoomType.MapToDomainEnum())))).ReturnsAsync(_rooms);

            var mockCommand = new CreateRoomCommand(_request.ConferenceId, "Judge", _request.RoomType.MapToDomainEnum(), false);
            _commandHandler.Setup(x => x.Handle(mockCommand));

            var returnedRoom =
                await _consultationService.GetAvailableConsultationRoomAsync(_request.ConferenceId, _request.RoomType.MapToDomainEnum());

            returnedRoom.Should().BeOfType<Room>();
            returnedRoom.Should().NotBeNull();
        }

        [Test]
        public async Task Should_Create_ConsultationRoom_If_None_Are_Available()
        {
            _queryHandler
                .Setup(x =>
                    x.Handle<GetAvailableRoomByRoomTypeQuery, List<Room>>(It.IsAny<GetAvailableRoomByRoomTypeQuery>()))
                .ReturnsAsync(new List<Room>());

            var consultationRoomParams = new CreateConsultationRoomParams
            {
                Room_label_prefix = _request.RoomType.ToString()
            };

            _kinlyApiClient
                .Setup(x => x.CreateConsultationRoomAsync(It.IsAny<string>(), It.IsAny<CreateConsultationRoomParams>()))
                .ReturnsAsync(new CreateConsultationRoomResponse() {Room_label = "Label"});

            var returnedRoom =
                await _consultationService.GetAvailableConsultationRoomAsync(_request.ConferenceId, _request.RoomType.MapToDomainEnum());

            _kinlyApiClient.Verify(x => x.CreateConsultationRoomAsync(It.Is<string>(
                y => y.Equals(_request.ConferenceId.ToString())), It.Is<CreateConsultationRoomParams>(
                y => y.Room_label_prefix.Equals(consultationRoomParams.Room_label_prefix))), Times.Once);
            returnedRoom.Should().BeOfType<Room>();
            returnedRoom.Should().NotBeNull();
        }

        [Test]
        public async Task Should_Successfully_Transfer_Participant_To_ConsultationRoom()
        {
            _queryHandler.Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(TestConference);

            var room = _rooms.First(x => x.ConferenceId.Equals(_request.ConferenceId));
            var participant =
                TestConference.Participants.First(x => x.Id.Equals(_request.RequestedBy));
            await _consultationService.ParticipantTransferToRoomAsync(_request.ConferenceId, _request.RequestedBy,
                room.Label);

            var request = new TransferParticipantParams
            {
                From = participant.GetCurrentRoom().ToString(),
                To = room.Label,
                Part_id = participant.Id.ToString()
            };

            _kinlyApiClient.Verify(x => x.TransferParticipantAsync(It.Is<string>(
                    y => y.Equals(_request.ConferenceId.ToString())), It.Is<TransferParticipantParams>(
                    y => y.From.Equals(request.From) && y.To.Equals(request.To) && y.Part_id.Equals(request.Part_id))),
                Times.Once);
        }

        private StartConsultationRequest RequestBuilder()
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

        private List<Room> CreateTestRooms(StartConsultationRequest request)
        {
            var rooms = new List<Room>();
            var room1 = new Room(request.ConferenceId, "Judge", request.RoomType.MapToDomainEnum(), false);
            var room2 = new Room(Guid.NewGuid(), "Waiting", VirtualCourtRoomType.JudgeJOH, false);

            rooms.Add(room1);
            rooms.Add(room2);

            return rooms;
        }

        [Test]
        public async Task should_remove_a_participant_in_room()
        {
            var participantId = TestConference.Participants[0].Id;
            var _fromRoom = "ConsultationRoom";
            var _toRoom = "WaitingRoom";
            await _consultationService.LeaveConsultationAsync(TestConference.Id, participantId, _fromRoom, _toRoom);

            _kinlyApiClient.Verify(x =>
                    x.TransferParticipantAsync(TestConference.Id.ToString(),
                        It.Is<TransferParticipantParams>(r =>
                            r.From == "ConsultationRoom" &&
                            r.To == "WaitingRoom"
                        )
                    )
                , Times.Exactly(1));
        }

        [Test]
        public async Task Should_Create_New_ConsultationRoom()
        {
            var mockCommand = new CreateRoomCommand(_request.ConferenceId, "Judge", _request.RoomType.MapToDomainEnum(), false);
            _commandHandler.Setup(x => x.Handle(mockCommand));
            var consultationRoomParams = new CreateConsultationRoomParams
            {
                Room_label_prefix = _request.RoomType.ToString()
            };

            _kinlyApiClient
                .Setup(x => x.CreateConsultationRoomAsync(It.IsAny<string>(), It.IsAny<CreateConsultationRoomParams>()))
                .ReturnsAsync(new CreateConsultationRoomResponse() { Room_label = "Label" });

            var returnedRoom =
                await _consultationService.CreateNewConsultationRoomAsync(_request.ConferenceId, _request.RoomType.MapToDomainEnum());

            _kinlyApiClient.Verify(x => x.CreateConsultationRoomAsync(It.Is<string>(
                y => y.Equals(_request.ConferenceId.ToString())), It.Is<CreateConsultationRoomParams>(
                y => y.Room_label_prefix.Equals(consultationRoomParams.Room_label_prefix))), Times.Once);
            returnedRoom.Should().BeOfType<Room>();
            returnedRoom.Should().NotBeNull();
        }
    }
}
