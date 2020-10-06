using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Queries;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers;
using VideoApi.Events.Models;

namespace VideoApi.UnitTests.Events
{
    public class PrivateConsultationRejectedEventHandlerTests : EventHandlerTestBase
    {
        private PrivateConsultationRejectedEventHandler _eventHandler;

        [Test] public async Task should_add_judge_task_if_not_already_todo()
        {
            _eventHandler = new PrivateConsultationRejectedEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object);
            
            var conference = TestConference;
            var participantForEvent = conference.GetParticipants().First();
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.PrivateConsultationRejected,
                EventId = Guid.NewGuid().ToString(),
                ParticipantId = participantForEvent.Id,
                ConferenceId = conference.Id,
                TimeStampUtc = DateTime.UtcNow,
                Reason = "Judge rejected private consultation"
            };

            var existingDoneTask = new VideoApi.Domain.Task(conference.Id, participantForEvent.Id,
                "Judge rejected private consultation", TaskType.Judge);
            existingDoneTask.CompleteTask("test");
            var tasks = new List<VideoApi.Domain.Task> {existingDoneTask};

            QueryHandlerMock.Setup(x => x.Handle<GetTasksForConferenceQuery, List<VideoApi.Domain.Task>>(
                It.IsAny<GetTasksForConferenceQuery>())).ReturnsAsync(tasks);

            await _eventHandler.HandleAsync(callbackEvent);
            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<AddTaskCommand>(c =>
                    c.TaskType == TaskType.Judge && c.OriginId == participantForEvent.Id &&
                    c.ConferenceId == conference.Id && c.Body == callbackEvent.Reason)), Times.Once);
        }

        [Test]
        public async Task should_not_add_rejection_task_if_already_present()
        {
            _eventHandler = new PrivateConsultationRejectedEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object);
            
            var conference = TestConference;
            var participantForEvent = conference.GetParticipants().First();
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.PrivateConsultationRejected,
                EventId = Guid.NewGuid().ToString(),
                ParticipantId = participantForEvent.Id,
                ConferenceId = conference.Id,
                TimeStampUtc = DateTime.UtcNow,
                Reason = "Judge rejected private consultation"
            };
            
            var existingDoneTask = new VideoApi.Domain.Task(conference.Id, participantForEvent.Id,
                "Judge rejected private consultation", TaskType.Judge);
            var tasks = new List<VideoApi.Domain.Task> {existingDoneTask};

            QueryHandlerMock.Setup(x => x.Handle<GetTasksForConferenceQuery, List<VideoApi.Domain.Task>>(
                It.IsAny<GetTasksForConferenceQuery>())).ReturnsAsync(tasks);
            
            await _eventHandler.HandleAsync(callbackEvent);
            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<AddTaskCommand>(c =>
                    c.TaskType == TaskType.Judge && c.OriginId == participantForEvent.Id &&
                    c.ConferenceId == conference.Id && c.Body == callbackEvent.Reason)), Times.Never);
        }
    }
}
