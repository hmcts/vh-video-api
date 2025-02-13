using System;
using System.Threading.Tasks;
using Moq;
using VideoApi.DAL.Commands;
using VideoApi.Domain.Enums;
using VideoApi.Events.Exceptions;
using VideoApi.Events.Handlers;
using VideoApi.Events.Models;

namespace VideoApi.UnitTests.Events;

public class TelephoneDisconnectedHandlerTests : EventHandlerTestBase<TelephoneDisconnectedEventHandler>
{
    [Test]
    public async Task Should_remove_telephone_participant()
    {
        var conference = TestConference;
        var telephoneParticipant = TestConference.GetTelephoneParticipants()[0];

        var callbackEvent = new CallbackEvent
        {
            EventType = EventType.TelephoneDisconnected,
            EventId = Guid.NewGuid().ToString(),
            ConferenceId = conference.Id,
            ParticipantId = telephoneParticipant.Id,
            TimeStampUtc = DateTime.UtcNow,
            Phone = telephoneParticipant.TelephoneNumber,
            Reason = "Telephone User Disconnected"
        };
        var updateStatusCommand = new RemoveTelephoneParticipantCommand(conference.Id, telephoneParticipant.Id);
        CommandHandlerMock.Setup(x => x.Handle(updateStatusCommand));

        await _sut.HandleAsync(callbackEvent);

        CommandHandlerMock.Verify(
            x => x.Handle(It.Is<RemoveTelephoneParticipantCommand>(command =>
                command.ConferenceId == conference.Id && command.TelephoneParticipantId == telephoneParticipant.Id)), Times.Once);
    }
    
    [Test]
    public void should_throw_exception_if_telephone_participant_has_been_updated_since_callback_received()
    {
        var conference = TestConference;
        var telephoneParticipant = TestConference.GetTelephoneParticipants()[0];
        telephoneParticipant.UpdateStatus(TelephoneState.Connected);
        
        var callbackEvent = new CallbackEvent
        {
            EventType = EventType.TelephoneDisconnected,
            EventId = Guid.NewGuid().ToString(),
            ConferenceId = conference.Id,
            ParticipantId = telephoneParticipant.Id,
            TimeStampUtc = DateTime.UtcNow.AddSeconds(-1),
            Phone = telephoneParticipant.TelephoneNumber,
            Reason = "Telephone User Disconnected"
        };
        
        var exception = Assert.ThrowsAsync<UnexpectedEventOrderException>(() => _sut.HandleAsync(callbackEvent));
        Assert.That(exception!.CallbackEvent, Is.EqualTo(callbackEvent));
        Assert.That(exception!.InnerException!.Message,
            Is.EqualTo($"TelephoneParticipant {telephoneParticipant.Id} has already been updated since this event {callbackEvent.EventType} with the time {callbackEvent.TimeStampUtc}. Current Status: {telephoneParticipant.State}"));
    }
}
