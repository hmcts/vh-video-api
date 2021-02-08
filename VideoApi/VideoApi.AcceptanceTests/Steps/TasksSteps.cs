using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AcceptanceTests.Common.Api.Helpers;
using FizzWare.NBuilder;
using FluentAssertions;
using TechTalk.SpecFlow;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.Domain.Enums;
using static Testing.Common.Helper.ApiUriFactory.EventsEndpoints;
using static Testing.Common.Helper.ApiUriFactory.TaskEndpoints;
using VideoApi.AcceptanceTests.Helpers;

namespace VideoApi.AcceptanceTests.Steps
{
    [Binding]
    public sealed class TaskSteps
    {
        private readonly TestContext _context;
        private readonly ScenarioContext _scenarioContext;
        private const string UpdatedBy = "AutomationUpdateUser";
        private static string _addTaskRequest = "AddTaskRequest";

        public TaskSteps(TestContext injectedContext, ScenarioContext scenarioContext)
        {
            _context = injectedContext;
            _scenarioContext = scenarioContext;
        }

        [Given(@"The conference has a pending task")]
        public void GivenTheConferenceHasATask()
        {
            var request = Builder<ConferenceEventRequest>.CreateNew()
                .With(x => x.ConferenceId = _context.Test.ConferenceResponse.Id.ToString())
                .With(x => x.ParticipantId = _context.Test.ConferenceResponse.Participants.First().Id.ToString())
                .With(x => x.EventId = Guid.NewGuid().ToString())
                .With(x => x.EventType = EventType.MediaPermissionDenied)
                .With(x => x.TransferFrom = RoomType.WaitingRoom.ToString())
                .With(x => x.TransferTo = RoomType.WaitingRoom.ToString())
                .With(x => x.Reason = "Automated")
                .With(x => x.Phone = null)
                .Build();
            _context.Request = _context.Post(Event, request);
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            _context.Response.IsSuccessful.Should().Be(true);
        }

        [Given(@"I have a valid get tasks request")]
        public void GivenIHaveAValidGetTasksRequest()
        {
            _context.Request = _context.Get(GetTasks(_context.Test.ConferenceResponse.Id));
        }

        [Given(@"I have a valid update task request")]
        public void GivenIHaveAValidUpdateTaskRequest()
        {
            var request = new UpdateTaskRequest {UpdatedBy = UpdatedBy};
            _context.Request = _context.Patch(UpdateTaskStatus(_context.Test.ConferenceResponse.Id, _context.Test.TaskId), request);
        }

        [Given(@"I have add task to a conference request with a (.*) conference id")]
        public void GivenIHaveAddTaskToAConferenceRequestWithAValidConferenceId(Scenario scenario)
        {
            var conferenceId = _context.Test.ConferenceResponse.Id;
            var participantId = _context.Test.ConferenceResponse.Participants.First(x => x.UserRole == UserRole.Individual).Id;
            var addTaskRequest = new AddTaskRequest { ParticipantId = participantId, Body = "Witness dismissed", TaskType = TaskType.Participant };
            _scenarioContext.Add(_addTaskRequest, addTaskRequest);

            switch (scenario)
            {
                case Scenario.Valid:
                    break;
                case Scenario.Invalid:
                    conferenceId = Guid.NewGuid();
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
            _context.Request = _context.Post(AddTask(conferenceId), addTaskRequest);
        }

        [Then(@"the tasks are retrieved")]
        public void ThenTheTaskIsRetrieved()
        {
            var tasks = RequestHelper.Deserialise<List<TaskResponse>>(_context.Response.Content);
            tasks.Should().NotBeNull();
            tasks.First().Id.Should().BeGreaterThan(-1);
            tasks.First().Created.Should().BeBefore(DateTime.Now);
            tasks.First().Type.Should().Be(TaskType.Participant);
            tasks.First().Body.Should().Contain("Media blocked");
            _context.Test.TaskId = tasks.First().Id;
        }

        [Then(@"the task is updated")]
        public void ThenTheStatusIsUpdated()
        {
            var task = RequestHelper.Deserialise<TaskResponse>(_context.Response.Content);
            task.Updated.HasValue.Should().BeTrue();
            task.UpdatedBy.Should().Be(UpdatedBy);
            task.Status.Should().Be(TaskStatus.Done);
        }

        [Then(@"the task should be added")]
        public void ThenTheTaskShouldBeAdded()
        {
            _context.Request = _context.Get(GetTasks(_context.Test.ConferenceResponse.Id));
            _context.Response = _context.Client().Execute(_context.Request);
            var tasks = RequestHelper.Deserialise<List<TaskResponse>>(_context.Response.Content);
            var taskRequested = _scenarioContext.Get<AddTaskRequest>(_addTaskRequest);
            var taskSaved = tasks.FirstOrDefault(t => t.OriginId == taskRequested.ParticipantId);

            taskSaved.Should().NotBeNull();
            taskSaved.Body.Should().Be(taskRequested.Body);
            taskSaved.OriginId.Should().Be(taskRequested.ParticipantId);
            taskSaved.Status.Should().Be(TaskStatus.ToDo);
            taskSaved.Type.Should().Be(taskRequested.TaskType);
        }
    }
}
