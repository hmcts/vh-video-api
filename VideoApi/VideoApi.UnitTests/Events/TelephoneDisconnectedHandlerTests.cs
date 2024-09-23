using System;
using System.Threading.Tasks;
using Moq;
using VideoApi.DAL.Commands;
using VideoApi.Domain.Enums;
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
}
