using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Common.Helpers;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.IntegrationTests.Contexts;
using VideoApi.IntegrationTests.Helper;
using Task = VideoApi.Domain.Task;

namespace VideoApi.IntegrationTests.Steps
{
    [Binding]
    public class TaskSteps : StepsBase
    {
        private readonly ConferenceTestContext _conferenceTestContext;
        private readonly TaskTestContext _taskTestContext;
        private readonly TaskEndpoints _endpoints = new ApiUriFactory().TaskEndpoints;

        public TaskSteps(ApiTestContext apiTestContext, ConferenceTestContext conferenceTestContext,
            TaskTestContext taskTestContext) : base(apiTestContext)
        {
            _conferenceTestContext = conferenceTestContext;
            _taskTestContext = taskTestContext;
        }

        [Given(@"I have a (.*) get tasks request")]
        [Given(@"I have an (.*) get tasks request")]
        public async System.Threading.Tasks.Task GivenIHaveAGetTasksRequest(Scenario scenario)
        {
            Guid conferenceId;
            switch (scenario)
            {
                case Scenario.Valid:
                    var seededConference = await SeedConferenceWithTasks();
                    _conferenceTestContext.SeededConference = seededConference;
                    TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
                    ApiTestContext.NewConferenceId = seededConference.Id;
                    conferenceId = seededConference.Id;
                    break;
                case Scenario.Nonexistent:
                    conferenceId = Guid.NewGuid();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            ApiTestContext.Uri = _endpoints.GetTasks(conferenceId);
            ApiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Then(@"the list of tasks should be retrieved")]
        public async System.Threading.Tasks.Task ThenTheListOfTasksShouldBeRetrieved()
        {
            var json = await ApiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var tasks = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<TaskResponse>>(json);
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
            var json = await ApiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var updatedTask = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<TaskResponse>(json);
            updatedTask.Should().NotBeNull();
            updatedTask.Updated.Should().NotBeNull();
            updatedTask.UpdatedBy.Should().Be(_taskTestContext.UpdateTaskRequest.UpdatedBy);
            updatedTask.Id.Should().Be(_taskTestContext.TaskToUpdate.Id);
        }

        [Given(@"I have a (.*) update task request")]
        [Given(@"I have an (.*) update task request")]
        public async System.Threading.Tasks.Task GivenIHaveAUpdateTaskRequest(Scenario scenario)
        {
            var seededConference = await SeedConferenceWithTasks();
            _conferenceTestContext.SeededConference = seededConference;
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            ApiTestContext.NewConferenceId = seededConference.Id;

            var conferenceId = seededConference.Id;
            long taskId;
            var request = new UpdateTaskRequest
            {
                UpdatedBy = seededConference.Participants
                    .First(x => x.UserRole == UserRole.Individual).Username
            };
            _taskTestContext.UpdateTaskRequest = request;
            switch (scenario)
            {
                case Scenario.Valid:
                    var task = seededConference.Tasks.First(x => x.Type == TaskType.Participant);
                    _taskTestContext.TaskToUpdate = task;
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

            ApiTestContext.Uri = _endpoints.UpdateTaskStatus(conferenceId, taskId);
            ApiTestContext.HttpMethod = HttpMethod.Patch;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            ApiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        private async Task<Conference> SeedConferenceWithTasks()
        {
            const string body = "Automated Test Complete Task";
            const string updatedBy = "test@automated.com";

            var conference = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Individual, "Claimant")
                .WithParticipant(UserRole.Judge, "Judge")
                .Build();

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

            return await ApiTestContext.TestDataManager.SeedConference(conference);
        }
    }
}