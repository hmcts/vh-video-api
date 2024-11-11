using System;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers;
using VideoApi.Events.Models;

namespace VideoApi.UnitTests.Events
{
    public class RecordingConnectionEventHandlerTests : EventHandlerTestBase<RecordingConnectionEventHandler>
    {
        [Test]
        public void Should_trigger_event_recording_failed()
        {
            
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.RecordingConnectionFailed,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = TestConference.Id,
                TimeStampUtc = DateTime.UtcNow
            };

            Assert.DoesNotThrowAsync(async () => await _sut.HandleAsync(callbackEvent));
        }
    }
}
