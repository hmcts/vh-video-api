using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Testing.Common.Extensions;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Contract.Enums;
using VideoApi.DAL.Commands;
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
            var videoPlatformServiceMock = _mocker.Mock<IVideoPlatformService>();
            var supplierPlatformServiceFactory = _mocker.Mock<ISupplierPlatformServiceFactory>();
            supplierPlatformServiceFactory.Setup(x => x.Create(It.IsAny<Supplier>())).Returns(videoPlatformServiceMock.Object);

            var conference = TestConference;
            conference.UpdateConferenceStatus(ConferenceState.InSession);
            const Supplier supplier = Supplier.Vodafone;
            conference.SetSupplier(supplier);
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
            
            videoPlatformServiceMock.Verify(x => x.TransferParticipantAsync(conference.Id, endpointForEvent.Id.ToString(), RoomType.WaitingRoom.ToString(), RoomType.HearingRoom.ToString()), Times.Once);
            supplierPlatformServiceFactory.Verify(x => x.Create(supplier), Times.Once);
        }
    }
}
