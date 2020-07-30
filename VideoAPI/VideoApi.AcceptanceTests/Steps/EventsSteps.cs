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

        public CallbackSteps(TestContext injectedContext)
        {
            _context = injectedContext;
        }

        [Given(@"I have a valid conference event request for a Judge with event type (.*)")]
        public void GivenIHaveAValidConferenceEventRequestForAJudge(EventType eventType)
        {
            var participant = _context.Test.ConferenceResponse.Participants.First(x => x.UserRole == UserRole.Judge);
            CreateConferenceEventRequest(participant, eventType);
        }
        
        [Given(@"I have a valid conference event request for event type (.*)")]
        public void GivenIHaveAValidConferenceEventRequest(EventType eventType)
        {
            var participant = _context.Test.ConferenceResponse.Participants.First(x => x.UserRole != UserRole.Judge);
            CreateConferenceEventRequest(participant, eventType);
        }

        private void CreateConferenceEventRequest(ParticipantDetailsResponse participant, EventType eventType)
        {
            _context.Test.ParticipantId = participant.Id;
            var request = Builder<ConferenceEventRequest>.CreateNew()
                .With(x => x.ConferenceId = _context.Test.ConferenceResponse.Id.ToString())
                .With(x => x.ParticipantId = participant.Id.ToString())
                .With(x => x.EventId = Guid.NewGuid().ToString())
                .With(x => x.EventType = eventType)
                .With(x => x.TransferFrom = RoomType.WaitingRoom)
                .With(x => x.TransferTo = RoomType.HearingRoom)
                .With(x => x.Reason = "Automated")
                .Build();

            _context.Request = _context.Post(EventsEndpoints.Event, request);
        }

        [Then(@"the status is updated")]
        public void ThenTheStatusIsUpdated()
        {
            _context.Request = _context.Get(ConferenceEndpoints.GetConferenceDetailsById(_context.Test.ConferenceResponse.Id));
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.IsSuccessful.Should().BeTrue("Conference details retrieved");
            var conference = RequestHelper.DeserialiseSnakeCaseJsonToResponse<ConferenceDetailsResponse>(_context.Response.Content);
            conference.Should().NotBeNull();
            var participant = conference.Participants.First(x => x.Id == _context.Test.ParticipantId);
            participant.CurrentStatus.Should().Be(ParticipantState.Available);
        }
    }
}
