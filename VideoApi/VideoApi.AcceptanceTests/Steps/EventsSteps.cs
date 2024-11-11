using System;
using System.Linq;
using FizzWare.NBuilder;
using TechTalk.SpecFlow;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.Contract.Enums;
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
        [Given(@"I have a valid conference event request for event type (.*)")]
        public void GivenIHaveAValidConferenceEventRequestForAJudge(EventType eventType)
        {
            var participantId = _context.Test.ConferenceResponse.Participants.First(x => x.UserRole == UserRole.Judge)
                .Id;
            CreateConferenceEventRequest(participantId, eventType);
        }
        
        private void CreateConferenceEventRequest(Guid participantId, EventType eventType)
        {
            _context.Test.ParticipantId = participantId;
            var request = Builder<ConferenceEventRequest>.CreateNew()
                .With(x => x.ConferenceId = _context.Test.ConferenceResponse.Id.ToString())
                .With(x => x.ParticipantId = participantId.ToString())
                .With(x => x.EventId = Guid.NewGuid().ToString())
                .With(x => x.EventType = eventType)
                .With(x => x.TransferFrom = RoomType.WaitingRoom.ToString())
                .With(x => x.TransferTo = RoomType.HearingRoom.ToString())
                .With(x => x.Reason = "Automated")
                .With(x => x.Phone = null)
                .With(x => x.ParticipantRoomId = null)
                .Build();

            _context.Request = TestContext.Post(EventsEndpoints.Event, request);
        }

        [Then(@"the status is updated")]
        public void ThenTheStatusIsUpdated()
        {
            _context.Request = TestContext.Get(ConferenceEndpoints.GetConferenceDetailsById(_context.Test.ConferenceResponse.Id));
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.IsSuccessful.Should().BeTrue("Conference details retrieved");
            var conference = ApiRequestHelper.Deserialise<ConferenceDetailsResponse>(_context.Response.Content);
            conference.Should().NotBeNull();
            var participant = conference.Participants.First(x => x.Id == _context.Test.ParticipantId);
            participant.CurrentStatus.Should().Be(ParticipantState.Available);
        }
    }
}
