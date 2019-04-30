using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers;
using VideoApi.Events.Models;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Events
{
    public class MediaPermissionDeniedEventHandlerTests : EventHandlerTestBase
    {
        private MediaPermissionDeniedEventHandler _eventHandler;

        [Test]
        public async Task should_call_command_handler_with_addtaskcommand_object()
        {
            _eventHandler = new MediaPermissionDeniedEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object,
                ServiceBusQueueClient, EventHubContextMock.Object);

            var conference = TestConference;
            var participantForEvent = conference.GetParticipants().First();
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Help,
                EventId = Guid.NewGuid().ToString(),
                ParticipantId = participantForEvent.Id,
                ConferenceId = conference.Id,
                TimeStampUtc = DateTime.UtcNow
            };

            var tasks = new List<VideoApi.Domain.Task>
            {
                new VideoApi.Domain.Task("Test", TaskType.Participant) {OriginId = Guid.NewGuid()}
            };

            QueryHandlerMock.Setup(x => x.Handle<GetIncompleteTasksForConferenceQuery, List<VideoApi.Domain.Task>>(
                It.IsAny<GetIncompleteTasksForConferenceQuery>())).ReturnsAsync(tasks);

            await _eventHandler.HandleAsync(callbackEvent);
            CommandHandlerMock.Verify(x => x.Handle<AddTaskCommand>(It.IsAny<AddTaskCommand>()), Times.Once);
        }

        [Test]
        public void should_throw_exception_when_conference_does_not_exist()
        {
            QueryHandlerMock
                .Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync((Conference) null);

            _eventHandler = new MediaPermissionDeniedEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object,
                ServiceBusQueueClient, EventHubContextMock.Object);

            var conference = TestConference;
            var participantForEvent = conference.GetParticipants().First();
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Help,
                EventId = Guid.NewGuid().ToString(),
                ParticipantId = participantForEvent.Id,
                ConferenceId = conference.Id,
                TimeStampUtc = DateTime.UtcNow
            };

            Assert.ThrowsAsync<ConferenceNotFoundException>(() =>
                _eventHandler.HandleAsync(callbackEvent));
        }
    }
}