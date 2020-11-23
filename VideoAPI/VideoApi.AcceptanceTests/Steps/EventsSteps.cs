using System;
using System.Linq;
using AcceptanceTests.Common.Api.Helpers;
using FizzWare.NBuilder;
using FluentAssertions;
using TechTalk.SpecFlow;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.Domain.Enums;
using static Testing.Common.Helper.ApiUriFactory;

namespace VideoApi.AcceptanceTests.Steps
{
    [Binding]
    public sealed class CallbackSteps
    {
        private readonly TestContext _context;
        private readonly EndPointsSteps _endPointsSteps;

        public CallbackSteps(TestContext injectedContext, EndPointsSteps endPointsSteps)
        {
            _context = injectedContext;
            _endPointsSteps = endPointsSteps;
        }

        [Given(@"I have a valid conference event request for a Judge with event type (.*)")]
        [Given(@"I have a valid conference event request for event type (.*)")]
        public void GivenIHaveAValidConferenceEventRequestForAJudge(EventType eventType)
        {
            var participantId = _context.Test.ConferenceResponse.Participants.First(x => x.UserRole == UserRole.Judge)
                .Id;
            CreateConferenceEventRequest(participantId, eventType);
        }
        
        [Given(@"I have a valid endpoint event request for event type (.*)")]
        public void GivenIHaveAValidEndpointEventRequestForAConference(EventType eventType)
        {
            var endpointId = _endPointsSteps.GetEndPoints().First().Id;
            CreateConferenceEventRequest(endpointId, eventType);
        }

        private void CreateConferenceEventRequest(Guid participantId, EventType eventType)
        {
            _context.Test.ParticipantId = participantId;
            var request = Builder<ConferenceEventRequest>.CreateNew()
                .With(x => x.ConferenceId = _context.Test.ConferenceResponse.Id.ToString())
                .With(x => x.ParticipantId = participantId.ToString())
                .With(x => x.EventId = Guid.NewGuid().ToString())
                .With(x => x.EventType = eventType)
                .With(x => x.TransferFrom = RoomType.WaitingRoom)
                .With(x => x.TransferTo = RoomType.HearingRoom)
                .With(x => x.Reason = "Automated")
                .With(x => x.Phone = null)
                .Build();

            _context.Request = _context.Post(EventsEndpoints.Event, request);
        }

        [Then(@"the status is updated")]
        public void ThenTheStatusIsUpdated()
        {
            _context.Request = _context.Get(ConferenceEndpoints.GetConferenceDetailsById(_context.Test.ConferenceResponse.Id));
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.IsSuccessful.Should().BeTrue("Conference details retrieved");
            var conference = RequestHelper.Deserialise<ConferenceDetailsResponse>(_context.Response.Content);
            conference.Should().NotBeNull();
            var participant = conference.Participants.First(x => x.Id == _context.Test.ParticipantId);
            participant.CurrentStatus.Should().Be(ParticipantState.Available);
        }
    }
}
