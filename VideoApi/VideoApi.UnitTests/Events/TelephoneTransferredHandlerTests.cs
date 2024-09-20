using System;
using System.Threading.Tasks;
using Moq;
using VideoApi.DAL.Commands;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers;
using VideoApi.Events.Models;

namespace VideoApi.UnitTests.Events;

public class TelephoneTransferredHandlerTests : EventHandlerTestBase<TelephoneTransferredEventHandler>
{
    [Test]
    public async Task Should_update_telephone_participant_to_hearing_room()
    {
        var conference = TestConference;
        var telephoneParticipant = TestConference.GetTelephoneParticipants()[0];

        var callbackEvent = new CallbackEvent
        {
            EventType = EventType.TelephoneTransfer,
            EventId = Guid.NewGuid().ToString(),
            ConferenceId = conference.Id,
            ParticipantId = telephoneParticipant.Id,
            TimeStampUtc = DateTime.UtcNow,
            Phone = telephoneParticipant.TelephoneNumber,
            Reason = "Telephone User Transferred",
            TransferFrom = RoomType.WaitingRoom,
            TransferTo = RoomType.HearingRoom
        };
        var updateStatusCommand = new UpdateTelephoneParticipantCommand(conference.Id, telephoneParticipant.Id,
            RoomType.WaitingRoom, TelephoneState.Disconnected);
        CommandHandlerMock.Setup(x => x.Handle(updateStatusCommand));

        await _sut.HandleAsync(callbackEvent);

        CommandHandlerMock.Verify(
            x => x.Handle(It.Is<UpdateTelephoneParticipantCommand>(command =>
                command.ConferenceId == conference.Id && command.TelephoneParticipantId == telephoneParticipant.Id &&
                command.Room == RoomType.HearingRoom && command.State == TelephoneState.Connected)), Times.Once);
    }
    
    [Test]
    public async Task Should_update_telephone_participant_to_waiting_room()
    {
        var conference = TestConference;
        var telephoneParticipant = TestConference.GetTelephoneParticipants()[0];

        var callbackEvent = new CallbackEvent
        {
            EventType = EventType.TelephoneTransfer,
            EventId = Guid.NewGuid().ToString(),
            ConferenceId = conference.Id,
            ParticipantId = telephoneParticipant.Id,
            TimeStampUtc = DateTime.UtcNow,
            Phone = telephoneParticipant.TelephoneNumber,
            Reason = "Telephone User Transferred",
            TransferFrom = RoomType.HearingRoom,
            TransferTo = RoomType.WaitingRoom
        };
        var updateStatusCommand = new UpdateTelephoneParticipantCommand(conference.Id, telephoneParticipant.Id,
            RoomType.HearingRoom, TelephoneState.Disconnected);
        CommandHandlerMock.Setup(x => x.Handle(updateStatusCommand));

        await _sut.HandleAsync(callbackEvent);

        CommandHandlerMock.Verify(
            x => x.Handle(It.Is<UpdateTelephoneParticipantCommand>(command =>
                command.ConferenceId == conference.Id && command.TelephoneParticipantId == telephoneParticipant.Id &&
                command.Room == RoomType.WaitingRoom && command.State == TelephoneState.Connected)), Times.Once);
    }
}
