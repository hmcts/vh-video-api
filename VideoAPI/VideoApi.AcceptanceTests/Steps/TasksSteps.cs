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

namespace VideoApi.AcceptanceTests.Steps
{
    [Binding]
    public sealed class TaskSteps : BaseSteps
    {
        private readonly TestContext _context;
        private const string UpdatedBy = "AutomationUpdateUser";

        public TaskSteps(TestContext injectedContext)
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
            _context.Request = _context.Post(Event, request);
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            _context.Response.IsSuccessful.Should().Be(true);
            _context.SetDefaultBearerToken();
        }

        [Given(@"I have a valid get tasks request")]
        public void GivenIHaveAValidGetTasksRequest()
        {
            _context.SetDefaultBearerToken();
            _context.Request = _context.Get(GetTasks(_context.NewConferenceId));
        }

        [Given(@"I have a valid update task request")]
        public void GivenIHaveAValidUpdateTaskRequest()
        {
            _context.SetDefaultBearerToken();
            var request = new UpdateTaskRequest {UpdatedBy = UpdatedBy};
            _context.Request = _context.Patch(UpdateTaskStatus(_context.NewConferenceId, _context.NewTaskId),
                request);
        }

        [Then(@"the tasks are retrieved")]
        public void ThenTheTaskIsRetrieved()
        {
            var tasks = RequestHelper.DeserialiseSnakeCaseJsonToResponse<List<TaskResponse>>(_context.Response.Content);
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
            var task = RequestHelper.DeserialiseSnakeCaseJsonToResponse<TaskResponse>(_context.Response.Content);
            task.Updated.HasValue.Should().BeTrue();
            task.UpdatedBy.Should().Be(UpdatedBy);
            task.Status.Should().Be(TaskStatus.Done);
        }
    }
}
