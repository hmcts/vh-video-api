using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using TechTalk.SpecFlow;
using VideoApi.Common.Helpers;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.IntegrationTests.Contexts;
using VideoApi.IntegrationTests.Helper;
using Task = VideoApi.Domain.Task;
using static Testing.Common.Helper.ApiUriFactory.TaskEndpoints;

namespace VideoApi.IntegrationTests.Steps
{
    [Binding]
    public class TaskBaseSteps : BaseSteps
    {
        private readonly TestContext _context;

        public TaskBaseSteps(TestContext context)
        {
            _context = context;
        }

        [Given(@"I have a (.*) get tasks request")]
        [Given(@"I have an (.*) get tasks request")]
        public async System.Threading.Tasks.Task GivenIHaveAGetTasksRequest(Scenario scenario)
        {
            var conferenceId = _context.Test.Conference.Id;
            switch (scenario)
            {
                case Scenario.Valid:
                    break;
                case Scenario.Nonexistent:
                    conferenceId = Guid.NewGuid();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            _context.Uri = GetTasks(conferenceId);
            _context.HttpMethod = HttpMethod.Get;
        }

        [Then(@"the list of tasks should be retrieved")]
        public async System.Threading.Tasks.Task ThenTheListOfTasksShouldBeRetrieved()
        {
            var json = await _context.ResponseMessage.Content.ReadAsStringAsync();
            var tasks = RequestHelper.DeserialiseSnakeCaseJsonToResponse<List<TaskResponse>>(json);
            tasks.Should().NotBeNullOrEmpty();
            tasks.Should().BeInDescendingOrder(x => x.Created);
            foreach (var task in tasks)
            {
                task.Id.Should().BeGreaterThan(0);
                task.Body.Should().NotBeNullOrWhiteSpace();
                task.Type.Should().BeOfType<TaskType>();
            }
        }

        [Then(@"the task should be retrieved with updated details")]
        public async System.Threading.Tasks.Task ThenTheTaskShouldBeRetrievedWithUpdatedDetails()
        {
            var json = await _context.ResponseMessage.Content.ReadAsStringAsync();
            var updatedTask = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<TaskResponse>(json);
            updatedTask.Should().NotBeNull();
            updatedTask.Updated.Should().NotBeNull();
            updatedTask.UpdatedBy.Should().Be(_context.Test.UpdateTaskRequest.UpdatedBy);
        }

        [Given(@"I have a (.*) update task request")]
        [Given(@"I have an (.*) update task request")]
        public void GivenIHaveAUpdateTaskRequest(Scenario scenario)
        {
            AddTasksToConference(_context.Test.Conference);
            var conferenceId = _context.Test.Conference.Id;
            long taskId;
            var request = new UpdateTaskRequest
            {
                UpdatedBy = _context.Test.Conference.Participants.First(x => x.UserRole == UserRole.Individual).Username
            };
            _context.Test.UpdateTaskRequest = request;
            switch (scenario)
            {
                case Scenario.Valid:
                    var task = _context.Test.Conference.Tasks.First(x => x.Type == TaskType.Participant);
                    taskId = task.Id;
                    break;
                case Scenario.Invalid:
                    taskId = 0;
                    break;
                case Scenario.Nonexistent:
                    taskId = 111111;
                    conferenceId = Guid.NewGuid();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            _context.Uri = UpdateTaskStatus(conferenceId, taskId);
            _context.HttpMethod = HttpMethod.Patch;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        private static void AddTasksToConference(Conference conference)
        {
            const string body = "Automated Test Complete Task";
            const string updatedBy = "test@automated.com";

            var judge = conference.GetParticipants().First(x => x.UserRole == UserRole.Judge);
            var individual = conference.GetParticipants().First(x => x.UserRole == UserRole.Individual);

            var judgeTaskDone = new Task(judge.Id, body, TaskType.Judge);
            judgeTaskDone.CompleteTask(updatedBy);
            var participantTaskDone = new Task(individual.Id, body, TaskType.Participant);
            participantTaskDone.CompleteTask(updatedBy);
            var hearingTaskDone = new Task(conference.Id, body, TaskType.Hearing);
            hearingTaskDone.CompleteTask(updatedBy);

            conference.AddTask(judge.Id, TaskType.Judge, body);
            conference.AddTask(individual.Id, TaskType.Participant, body);
            conference.AddTask(conference.Id, TaskType.Hearing, body);
            conference.AddTask(individual.Id, TaskType.Participant, body);

            conference.GetTasks()[0].CompleteTask(updatedBy);
        }
    }
}
