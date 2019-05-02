using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FluentAssertions;
using TechTalk.SpecFlow;
using Testing.Common.Assertions;
using Testing.Common.Helper;
using Testing.Common.Helper.Builders.Api;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.Contract.Responses;
using VideoApi.Domain.Enums;

namespace VideoApi.AcceptanceTests.Steps
{
    [Binding]
    public sealed class TasksSteps : BaseSteps
    {
        private readonly TestContext _context;
        private readonly ScenarioContext _scenarioContext;
        private readonly TaskEndpoints _endpoints = new ApiUriFactory().TaskEndpoints;

        public TasksSteps(TestContext injectedContext, ScenarioContext scenarioContext)
        {
            _context = injectedContext;
            _scenarioContext = scenarioContext;
        }

        [Given(@"I have a get tasks request with a valid conference id")]
        public void GivenIHaveAGetTasksRequestWithAValidConferenceId()
        {
            _context.Request = _context.Get(_endpoints.GetPendingTasks(_context.NewConference.Id));
        }

        [Given(@"I have a valid update task request")]
        public void GivenIHaveAValidCreateTaskRequest()
        {
            var callbackSteps = new CallbackSteps(_context, _scenarioContext);
            callbackSteps.GivenIHaveAValidConferenceEventRequest(EventType.MediaPermissionDenied);
            CreateTaskRequest();
        }

        private void CreateTaskRequest()
        {
           // _context.Request = _context.Patch(_endpoints.UpdateTaskStatus(_context.NewConference.Id, _context.CallbackEvent.EventId));
        }
    }
}
