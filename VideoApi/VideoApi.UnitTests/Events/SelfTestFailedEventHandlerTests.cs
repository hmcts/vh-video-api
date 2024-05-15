using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
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
    public class SelfTestFailedEventHandlerTests : EventHandlerTestBase<SelfTestFailedEventHandler>
    {
        [Test]
        public async Task Should_call_command_handler_with_addtaskcommand_object()
        {
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
                new VideoApi.Domain.Task(Guid.NewGuid(), Guid.NewGuid(), "Test", TaskType.Participant)
            };

            QueryHandlerMock.Setup(x => x.Handle<GetTasksForConferenceQuery, List<VideoApi.Domain.Task>>(
                It.IsAny<GetTasksForConferenceQuery>())).ReturnsAsync(tasks);

            await _sut.HandleAsync(callbackEvent);
            CommandHandlerMock.Verify(x => x.Handle(It.IsAny<AddTaskCommand>()), Times.Once);
        }

        [Test]
        public async Task Should_not_call_command_handler_with_addtaskcommand_object_if_a_task_exists()
        {
            var conference = TestConference;
            var participantForEvent = conference.GetParticipants().First();
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Help,
                EventId = Guid.NewGuid().ToString(),
                ParticipantId = participantForEvent.Id,
                ConferenceId = conference.Id,
                TimeStampUtc = DateTime.UtcNow,
                Reason = "Test"
            };

            var tasks = new List<VideoApi.Domain.Task>
            {
                new VideoApi.Domain.Task(conference.Id,participantForEvent.Id, "Test", TaskType.Participant)
            };

            QueryHandlerMock.Setup(x => x.Handle<GetTasksForConferenceQuery, List<VideoApi.Domain.Task>>(
                It.IsAny<GetTasksForConferenceQuery>())).ReturnsAsync(tasks);

            await _sut.HandleAsync(callbackEvent);
            CommandHandlerMock.Verify(x => x.Handle(It.IsAny<AddTaskCommand>()), Times.Never);
        }

        [Test]
        public void Should_throw_exception_when_conference_does_not_exist()
        {
            QueryHandlerMock
                .Setup(x => x.Handle<GetConferenceByIdForEventQuery, Conference>(It.IsAny<GetConferenceByIdForEventQuery>()))
                .ReturnsAsync((Conference) null);

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
                _sut.HandleAsync(callbackEvent));
        }
    }
}
