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
    public sealed class CallbackSteps : BaseSteps
    {
        private readonly TestContext _context;
        private readonly ScenarioContext _scenarioContext;
        private readonly CallbackEndpoints _endpoints = new ApiUriFactory().CallbackEndpoints;
        private const string PreviousStateKey = "PreviousState";

        public CallbackSteps(TestContext injectedContext, ScenarioContext scenarioContext)
        {
            _context = injectedContext;
            _scenarioContext = scenarioContext;
        }

        [Given(@"I have a valid conference event request")]
        public void GivenIHaveAValidConferenceEventRequest()
        {
            _scenarioContext.Add(PreviousStateKey, _context.NewConference.CurrentStatus ?? null);
            var request = Builder<ConferenceEventRequest>.CreateNew()
                .With(x => x.ConferenceId = _context.NewConferenceId.ToString())
                .With(x => x.ParticipantId = _context.NewConference.Participants.First().Id.ToString())
                .With(x => x.EventId = Guid.NewGuid().ToString())
                .With(x => x.EventType = EventType.Transfer)
                .With(x => x.TransferFrom = RoomType.WaitingRoom)
                .With(x => x.TransferTo = RoomType.ConsultationRoom1)
                .With(x => x.Reason = "Automated")
                .Build();
            _context.Request = _context.Post(_endpoints.Event, request);
        }

        [Then(@"the status is updated")]
        public void ThenTheStatusIsUpdated()
        {
            var endpoints = new ApiUriFactory().ConferenceEndpoints;
            _context.Request = _context.Get(endpoints.GetConferenceDetailsById(_context.NewConferenceId));
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.IsSuccessful.Should().BeTrue();
            var conference = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<ConferenceDetailsResponse>(_context.Response.Content);
            conference.Should().NotBeNull();
            conference.CurrentStatus.Should().NotBe(_scenarioContext.Get<string>(PreviousStateKey));
        }
    }
}
