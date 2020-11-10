using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using TechTalk.SpecFlow;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.Domain.Enums;
using VideoApi.IntegrationTests.Contexts;
using VideoApi.IntegrationTests.Helper;
using static Testing.Common.Helper.ApiUriFactory.TaskEndpoints;
using Alert = VideoApi.Domain.Task;

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

        [Given(@"A conference has tasks")]
        public async Task GivenAConferenceHasTasks()
        {
            var conferenceId = _context.Test.Conference.Id;
            var participantId = _context.Test.Conference.GetParticipants().First().Id;
            var judgeId = _context.Test.Conference.GetJudge().Id;

            var alert1 = new Alert(conferenceId, conferenceId, "Automated Test", TaskType.Hearing);
            var alert2 = new Alert(conferenceId, participantId, "Automated Test", TaskType.Participant);
            var alert3 = new Alert(conferenceId, judgeId, "Automated Test", TaskType.Judge);

            _context.Test.Alerts = await _context.TestDataManager.SeedAlerts(new List<Alert>
            {
                alert1, alert2, alert3
            });
        }

        [Given(@"I have a (.*) get tasks request")]
        [Given(@"I have an (.*) get tasks request")]
        public void GivenIHaveAGetTasksRequest(Scenario scenario)
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
        public async Task ThenTheListOfTasksShouldBeRetrieved()
        {
            var json = await _context.Response.Content.ReadAsStringAsync();
            var tasks = RequestHelper.Deserialise<List<TaskResponse>>(json);
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
        public async Task ThenTheTaskShouldBeRetrievedWithUpdatedDetails()
        {
            var json = await _context.Response.Content.ReadAsStringAsync();
            var updatedTask = RequestHelper.Deserialise<TaskResponse>(json);
            updatedTask.Should().NotBeNull();
            updatedTask.Updated.Should().NotBeNull();
            updatedTask.UpdatedBy.Should().Be(_context.Test.UpdateTaskRequest.UpdatedBy);
        }

        [Given(@"I have a (.*) update task request")]
        [Given(@"I have an (.*) update task request")]
        public void GivenIHaveAUpdateTaskRequest(Scenario scenario)
        {
            var conferenceId = _context.Test.Conference.Id;
            long taskId;
            _context.Test.UpdateTaskRequest = new UpdateTaskRequest
            {
                UpdatedBy = _context.Test.Conference.Participants.First(x => x.UserRole == UserRole.Individual).Username
            };
            switch (scenario)
            {
                case Scenario.Valid:
                    taskId = _context.Test.Alerts.First().Id;
                    break;
                case Scenario.Invalid:
                    taskId = 0;
                    break;
                case Scenario.Nonexistent:
                    taskId = _context.Test.Alerts.First().Id;
                    conferenceId = Guid.NewGuid();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            _context.Uri = UpdateTaskStatus(conferenceId, taskId);
            _context.HttpMethod = HttpMethod.Patch;
            var jsonBody = RequestHelper.Serialise(_context.Test.UpdateTaskRequest);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have a (.*) add task for a participant in a conference request")]
        [Given(@"I have an (.*) add task for a participant in a conference request")]
        public void GivenIHaveAValidAddTaskForAParticipantInAConferenceRequest(Scenario scenario)
        {
            var conferenceId = _context.Test.Conference.Id;
            var participantId = _context.Test.Conference.Participants.First(x => x.UserRole == UserRole.Individual).Id;
            var addTaskRequest = new AddTaskRequest { ParticipantId = participantId, Body = "Witness dismissed", TaskType = TaskType.Participant };
            switch (scenario)
            {
                case Scenario.Valid:
                    break;
                case Scenario.Invalid:
                    conferenceId = Guid.NewGuid();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
            _context.Uri = AddTask(conferenceId);
            _context.HttpMethod = HttpMethod.Post;
            var jsonBody = RequestHelper.Serialise(addTaskRequest);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }
    }
}
