using System;
using System.Linq;
using FizzWare.NBuilder;
using FluentAssertions;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.Domain.Enums;

namespace VideoApi.AcceptanceTests.Steps
{
    [Binding]
    public sealed class Tasksteps : BaseSteps
    {
        private readonly TestContext _context;
        private readonly ScenarioContext _scenarioContext;
        private readonly TaskEndpoints _endpoints = new ApiUriFactory().TaskEndpoints;
        private const string PreviousStateKey = "PreviousState";

        public Tasksteps(TestContext injectedContext, ScenarioContext scenarioContext)
        {
            _context = injectedContext;
            _scenarioContext = scenarioContext;
        }

        [Given(@"I have a valid get tasks request")]
        public void GivenIHaveAValidGetTasksRequest()
        {
            _context.Request = _context.Get(_endpoints.GetPendingTasks(_context.NewConferenceId));
        }

        [Given(@"I have a valid update task request")]
        public void GivenIHaveAValidUpdateTaskRequest()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"the task is retrieved")]
        public void ThenTheTaskIsRetrieved()
        {
            var tasks = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<TaskResponse>(_context.Response.Content);
            tasks.Should().NotBeNull();
        }

        [Then(@"the task is updated")]
        public void ThenTheStatusIsUpdated()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
