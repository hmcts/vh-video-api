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
using EventType = VideoApi.Domain.Enums.EventType;
using ParticipantState = VideoApi.Domain.Enums.ParticipantState;
using RoomType = VideoApi.Domain.Enums.RoomType;
using UserRole = VideoApi.Domain.Enums.UserRole;
using Supplier = VideoApi.Domain.Enums.Supplier;

namespace VideoApi.UnitTests.Events
{
    public class JoinedEventHandlerTests : EventHandlerTestBase<JoinedEventHandler>
    {
        [Test]
        public async Task Should_send_available_message_to_participants_and_service_bus_when_participant_joins()
        {
            var conference = TestConference;
            var participantForEvent = conference.GetParticipants().First(x => x.UserRole == UserRole.Individual);

            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Joined,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id,
                ParticipantId = participantForEvent.Id,
                TimeStampUtc = DateTime.UtcNow
            };
            var updateStatusCommand = new UpdateParticipantStatusAndRoomCommand(conference.Id, participantForEvent.Id,
                ParticipantState.Available, RoomType.WaitingRoom, null);
            CommandHandlerMock.Setup(x => x.Handle(updateStatusCommand));

            await _sut.HandleAsync(callbackEvent);

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateParticipantStatusAndRoomCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.ParticipantId == participantForEvent.Id &&
                    command.ParticipantState == ParticipantState.Available &&
                    command.Room == RoomType.WaitingRoom)), Times.Once);
        }
        
        [Test]
        public async Task Should_transfer_participant_to_hearing_room_when_conference_is_in_session()
        {
            var videoPlatformServiceMock = _mocker.Mock<IVideoPlatformService>();
            var supplierPlatformServiceFactory = _mocker.Mock<ISupplierPlatformServiceFactory>();
            supplierPlatformServiceFactory.Setup(x => x.Create(It.IsAny<Supplier>())).Returns(videoPlatformServiceMock.Object);
            
            var conference = TestConference;
            conference.UpdateConferenceStatus(ConferenceState.InSession);
            const Supplier supplier = Supplier.Vodafone;
            conference.SetSupplier(supplier);
            var participantForEvent = conference.GetParticipants().First(x => x.UserRole == UserRole.Individual);
            
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Joined,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id,
                ParticipantId = participantForEvent.Id,
                TimeStampUtc = DateTime.UtcNow
            };
            var updateStatusCommand = new UpdateParticipantStatusAndRoomCommand(conference.Id, participantForEvent.Id,
                ParticipantState.Available, RoomType.WaitingRoom, null);
            CommandHandlerMock.Setup(x => x.Handle(updateStatusCommand));

            await _sut.HandleAsync(callbackEvent);

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateParticipantStatusAndRoomCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.ParticipantId == participantForEvent.Id &&
                    command.ParticipantState == ParticipantState.Available &&
                    command.Room == RoomType.WaitingRoom)), Times.Once);
            
            videoPlatformServiceMock.Verify(x => x.TransferParticipantAsync(conference.Id, participantForEvent.Id.ToString(), RoomType.WaitingRoom.ToString(), RoomType.HearingRoom.ToString()), Times.Once);
            supplierPlatformServiceFactory.Verify(x => x.Create(supplier), Times.Once);
        }
    }
}
