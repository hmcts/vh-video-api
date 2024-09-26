using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers;
using VideoApi.Events.Models;
using VideoApi.Services.Contracts;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Events
{
    public class RecordingConnectionEventHandlerTests : EventHandlerTestBase<RecordingConnectionEventHandler>
    {
        [Test]
        public async Task Should_trigger_event_recording_failed()
        {
            
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.RecordingConnectionFailed,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = TestConference.Id,
                TimeStampUtc = DateTime.UtcNow
            };

            await _sut.HandleAsync(callbackEvent);
            
        }
    }
}
