using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Video.API.Services;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
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
            _queryHandler.Setup(x => x.Handle<GetAvailableRoomByRoomTypeQuery, List<Room>>(It.Is<GetAvailableRoomByRoomTypeQuery>(
                x => x.ConferenceId.Equals(_request.ConferenceId) && x.CourtRoomType.Equals(_request.RoomType)))).ReturnsAsync(_rooms);
            
            var mockCommand = new CreateRoomCommand(_request.ConferenceId, "Judge", _request.RoomType);
            _commandHandler.Setup(x => x.Handle(mockCommand));
            
            var returnedRoom = await _consultationService.GetAvailableConsultationRoomAsync(_request);

            returnedRoom.Should().BeOfType<Room>();
            returnedRoom.Should().NotBeNull();
        }
        
        [Test]
        public async Task Should_Create_ConsultationRoom_If_None_Are_Available()
        {
            _queryHandler.Setup(x => x.Handle<GetAvailableRoomByRoomTypeQuery, List<Room>>(It.IsAny<GetAvailableRoomByRoomTypeQuery>())).ReturnsAsync(new List<Room>());

            var consultationRoomParams = new CreateConsultationRoomParams
            {
                Room_label_prefix = _request.RoomType.ToString()
            };

            _kinlyApiClient
                .Setup(x => x.CreateConsultationRoomAsync(It.IsAny<string>(), It.IsAny<CreateConsultationRoomParams>()))
                .ReturnsAsync(new CreateConsultationRoomResponse(){ Room_label = "Label"});
            
            var returnedRoom = await _consultationService.GetAvailableConsultationRoomAsync(_request);
            
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
            await _consultationService.TransferParticipantToConsultationRoomAsync(_request, room);
            
            var request = new TransferParticipantParams
            {
                From = participant.GetCurrentRoom().ToString(),
                To = room.Label,
                Part_id = participant.Id.ToString()
            };
            
            _kinlyApiClient.Verify(x => x.TransferParticipantAsync(It.Is<string>(
                y => y.Equals(_request.ConferenceId.ToString())), It.Is<TransferParticipantParams>(
                y => y.From.Equals(request.From) && y.To.Equals(request.To) && y.Part_id.Equals(request.Part_id))), Times.Once);
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
                RoomType = VirtualCourtRoomType.JudgeJOH
            };
        }
        
        private List<Room> CreateTestRooms(StartConsultationRequest request)
        {
            var rooms = new List<Room>();
            var room1 = new Room(request.ConferenceId, "Judge", request.RoomType);
            var room2 = new Room(Guid.NewGuid(), "Waiting", VirtualCourtRoomType.JudgeJOH);
           
            rooms.Add(room1);
            rooms.Add(room2);

            return rooms;
        }
    }
}
