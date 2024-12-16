using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using VideoApi.DAL.Commands;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers;
using VideoApi.Events.Models;
using VideoApi.Services;
using VideoApi.Services.Contracts;
using ConferenceState = VideoApi.Domain.Enums.ConferenceState;
using EndpointState = VideoApi.Domain.Enums.EndpointState;
using EventType = VideoApi.Domain.Enums.EventType;
using ParticipantState = VideoApi.Domain.Enums.ParticipantState;
using RoomType = VideoApi.Domain.Enums.RoomType;
using Supplier = VideoApi.Domain.Enums.Supplier;

namespace VideoApi.UnitTests.Events
{
    public class EndpointJoinedHandlerTests : EventHandlerTestBase<EndpointJoinedEventHandler>
    {
        private Mock<IVideoPlatformService> _videoPlatformServiceMock;
        private Mock<ISupplierPlatformServiceFactory> _supplierPlatformServiceFactoryMock;
        
        [SetUp]
        public void SetUp()
        {
            _videoPlatformServiceMock = _mocker.Mock<IVideoPlatformService>();
            _supplierPlatformServiceFactoryMock = _mocker.Mock<ISupplierPlatformServiceFactory>();
            _supplierPlatformServiceFactoryMock.Setup(x => x.Create(It.IsAny<Supplier>())).Returns(_videoPlatformServiceMock.Object);
        }
        
        [Test]
        public async Task Should_update_endpoint_status_to_connected()
        {
            var conference = TestConference;
            var participantForEvent = conference.GetEndpoints().First();

            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.EndpointJoined,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id,
                ParticipantId = participantForEvent.Id,
                TimeStampUtc = DateTime.UtcNow
            };
            var updateStatusCommand = new UpdateEndpointStatusAndRoomCommand(conference.Id, participantForEvent.Id,
                EndpointState.Connected, RoomType.WaitingRoom, null);
            CommandHandlerMock.Setup(x => x.Handle(updateStatusCommand));

            await _sut.HandleAsync(callbackEvent);

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateEndpointStatusAndRoomCommand>(command =>
                    command.ConferenceId == conference.Id && command.EndpointId == participantForEvent.Id &&
                    command.Status == EndpointState.Connected && command.Room == RoomType.WaitingRoom)), Times.Once);
        }
        
        [Test]
        public async Task
            Should_transfer_endpoint_to_hearing_room_when_conference_is_in_session()
        {
            var conference = TestConference;
            conference.UpdateConferenceStatus(ConferenceState.InSession);
            var endpointForEvent = conference.GetEndpoints()[0];
            
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.EndpointJoined,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id,
                ParticipantId = endpointForEvent.Id,
                TimeStampUtc = DateTime.UtcNow
            };
            var updateStatusCommand = new UpdateParticipantStatusAndRoomCommand(conference.Id, endpointForEvent.Id,
                ParticipantState.Available, RoomType.WaitingRoom, null);
            CommandHandlerMock.Setup(x => x.Handle(updateStatusCommand));

            await _sut.HandleAsync(callbackEvent);

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateEndpointStatusAndRoomCommand>(command =>
                    command.ConferenceId == conference.Id && command.EndpointId == endpointForEvent.Id &&
                    command.Status == EndpointState.Connected && command.Room == RoomType.WaitingRoom)), Times.Once);
            
            _videoPlatformServiceMock.Verify(x => x.TransferParticipantAsync(conference.Id, endpointForEvent.Id.ToString(), RoomType.WaitingRoom.ToString(), RoomType.HearingRoom.ToString(), endpointForEvent.ConferenceRole), Times.Once);
            VerifySupplierUsed(TestConference.Supplier, Times.Exactly(1));
        }
    }
}
