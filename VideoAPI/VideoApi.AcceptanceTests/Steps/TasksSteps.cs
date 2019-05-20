using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private readonly TaskEndpoints _endpoints = new ApiUriFactory().TaskEndpoints;
        private readonly CallbackEndpoints _callbackEndpointsndpoints = new ApiUriFactory().CallbackEndpoints;
        private const string UpdatedBy = "AutomationUpdateUser";

        public Tasksteps(TestContext injectedContext)
        {
            _context = injectedContext;
        }

        [Given(@"The conference has a pending task")]
        public void GivenTheConferenceHasATask()
        {
            var request = Builder<ConferenceEventRequest>.CreateNew()
                .With(x => x.ConferenceId = _context.NewConferenceId.ToString())
                .With(x => x.ParticipantId = _context.NewConference.Participants.First().Id.ToString())
                .With(x => x.EventId = Guid.NewGuid().ToString())
                .With(x => x.EventType = EventType.MediaPermissionDenied)
                .With(x => x.TransferFrom = RoomType.WaitingRoom)
                .With(x => x.TransferTo = RoomType.WaitingRoom)
                .With(x => x.Reason = "Automated")
                .Build();
            _context.Request = _context.Post(_callbackEndpointsndpoints.Event, request);
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            _context.Response.IsSuccessful.Should().Be(true);
        }

        [Given(@"I have a valid get tasks request")]
        public void GivenIHaveAValidGetTasksRequest()
        {
            _context.Request = _context.Get(_endpoints.GetTasks(_context.NewConferenceId));
        }

        [Given(@"I have a valid update task request")]
        public void GivenIHaveAValidUpdateTaskRequest()
        {
            var request = new UpdateTaskRequest {UpdatedBy = UpdatedBy};
            _context.Request = _context.Patch(_endpoints.UpdateTaskStatus(_context.NewConferenceId, _context.NewTaskId), request);
        }

        [Then(@"the tasks are retrieved")]
        public void ThenTheTaskIsRetrieved()
        {
            var tasks = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<TaskResponse>>(_context.Response.Content);
            tasks.Should().NotBeNull();
            tasks.First().Id.Should().BeGreaterThan(-1);
            tasks.First().Created.Should().BeBefore(DateTime.Now);
            tasks.First().Type.Should().Be(TaskType.Participant);
            tasks.First().Body.Should().Contain("Media blocked");
            _context.NewTaskId = tasks.First().Id;
        }

        [Then(@"the task is updated")]
        public void ThenTheStatusIsUpdated()
        {
            var task = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<TaskResponse>(_context.Response.Content);
            task.Updated.HasValue.Should().BeTrue();
            task.UpdatedBy.Should().Be(UpdatedBy);
            task.Status.Should().Be(TaskStatus.Done);
        }
    }
}
