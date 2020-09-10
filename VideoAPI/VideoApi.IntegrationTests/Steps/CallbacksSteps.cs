using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using AcceptanceTests.Common.Api.Helpers;
using FizzWare.NBuilder;
using TechTalk.SpecFlow;
using VideoApi.Contract.Requests;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.IntegrationTests.Helper;
using static Testing.Common.Helper.ApiUriFactory;
using TestContext = VideoApi.IntegrationTests.Contexts.TestContext;

namespace VideoApi.IntegrationTests.Steps
{
    [Binding]
    public class CallbackSteps : BaseSteps
    {
        private readonly TestContext _context;
        public CallbackSteps(TestContext context)
        {
            _context = context;
        }

        [Given(@"I have a valid conference event request for event type (.*)")]
        public void GivenIHaveAnConferenceEventRequestForAnEventType(EventType eventType)
        {
            var request = BuildRequest(eventType, _context.Test.Conference);
            _context.Uri = EventsEndpoints.Event;
            _context.HttpMethod = HttpMethod.Post;
            var jsonBody = RequestHelper.Serialise(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have a (.*) conference event request")]
        [Given(@"I have an (.*) conference event request")]
        public void GivenIHaveAnConferenceEventRequest(Scenario scenario)
        {
            var request = scenario switch
            {
                Scenario.Nonexistent => BuildRequest(EventType.Transfer),
                Scenario.Invalid => BuildInvalidRequest(),
                _ => throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null)
            };

            _context.Uri = EventsEndpoints.Event;
            _context.HttpMethod = HttpMethod.Post;
            var jsonBody = RequestHelper.Serialise(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have a room transfer event request for a nonexistent participant")]
        public void GivenIRoomTransferEventRequestForNonExistentParticipant()
        {
            var request = BuildRequest(EventType.Transfer, _context.Test.Conference);
            request.ParticipantId = Guid.NewGuid().ToString();

            _context.Uri = EventsEndpoints.Event;
            _context.HttpMethod = HttpMethod.Post;
            var jsonBody = RequestHelper.Serialise(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        private ConferenceEventRequest BuildRequest(EventType eventType, Conference conference = null)
        {
            var request = Builder<ConferenceEventRequest>.CreateNew()
                .With(x => x.ConferenceId = Guid.NewGuid().ToString())
                .With(x => x.ParticipantId = Guid.NewGuid().ToString())
                .With(x => x.EventId = Guid.NewGuid().ToString())
                .With(x => x.EventType = eventType)
                .With(x => x.TransferFrom = RoomType.WaitingRoom)
                .With(x => x.TransferTo = RoomType.ConsultationRoom1)
                .With(x => x.Reason = "Automated")
                .Build();

            if (conference == null) return request;

            request.ConferenceId = conference.Id.ToString();
            var isEndpointEvent = eventType == EventType.EndpointJoined || eventType == EventType.EndpointDisconnected;
            var participantId = isEndpointEvent ? conference.GetEndpoints().First().Id: conference.GetParticipants().First().Id;
            request.ParticipantId = participantId.ToString();
            _context.Test.ParticipantId = participantId;
            return request;
        }

        private static ConferenceEventRequest BuildInvalidRequest()
        {
            var request = Builder<ConferenceEventRequest>.CreateNew()
                .With(x => x.ConferenceId = string.Empty)
                .With(x => x.ParticipantId = string.Empty)
                .With(x => x.EventId = string.Empty)
                .With(x => x.EventType = EventType.None)
                .With(x => x.Reason = "Automated")
                .Build();
            return request;
        }
    }
}
