using System;
using System.Linq;
using FizzWare.NBuilder;
using FluentAssertions;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.Common.Helpers;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.Domain.Enums;

namespace VideoApi.AcceptanceTests.Steps
{
    [Binding]
    public sealed class CallbackSteps : BaseSteps
    {
        private readonly TestContext _context;
        private readonly EventsEndpoints _endpoints = new ApiUriFactory().EventsEndpoints;

        public CallbackSteps(TestContext injectedContext)
        {
            _context = injectedContext;
        }

        [Given(@"I have a valid conference event request for event type (.*) for a Judge")]
        public void GivenIHaveAValidConferenceEventRequestForAJudge(EventType eventType)
        {
            var participant = _context.NewConference.Participants.First(x => x.UserRole == UserRole.Judge);
            CreateConferenceEventRequest(participant, eventType);
        }
        
        [Given(@"I have a valid conference event request for event type (.*)")]
        public void GivenIHaveAValidConferenceEventRequest(EventType eventType)
        {
            var participant = _context.NewConference.Participants.First(x => x.UserRole != UserRole.Judge);
            CreateConferenceEventRequest(participant, eventType);
        }

        private void CreateConferenceEventRequest(ParticipantDetailsResponse participant, EventType eventType)
        {
            _context.ParticipantId = participant.Id;
            var request = Builder<ConferenceEventRequest>.CreateNew()
                .With(x => x.ConferenceId = _context.NewConferenceId.ToString())
                .With(x => x.ParticipantId = participant.Id.ToString())
                .With(x => x.EventId = Guid.NewGuid().ToString())
                .With(x => x.EventType = eventType)
                .With(x => x.TransferFrom = RoomType.WaitingRoom)
                .With(x => x.TransferTo = RoomType.WaitingRoom)
                .With(x => x.Reason = "Automated")
                .Build();

            _context.Request = _context.Post(_endpoints.Event, request);
        }

        [Then(@"the status is updated")]
        public void ThenTheStatusIsUpdated()
        {
            _context.SetDefaultBearerToken();
            var endpoints = new ApiUriFactory().ConferenceEndpoints;
            _context.Request = _context.Get(endpoints.GetConferenceDetailsById(_context.NewConferenceId));
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.IsSuccessful.Should().BeTrue("Conference details retrieved");
            var conference =
                ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<ConferenceDetailsResponse>(
                    _context.Response.Content);
            conference.Should().NotBeNull();
            var participant = conference.Participants.First(x => x.Id == _context.ParticipantId);
            if (participant.UserRole == UserRole.Judge)
            {
                participant.CurrentStatus.Should().Be(ParticipantState.InHearing);
            }
            else
            {
                participant.CurrentStatus.Should().Be(ParticipantState.Available);
            }
        }
    }
}
