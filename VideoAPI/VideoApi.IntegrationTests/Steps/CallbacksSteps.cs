using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using NUnit.Framework;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using VideoApi.Contract.Requests;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.IntegrationTests.Contexts;
using VideoApi.IntegrationTests.Helper;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Steps
{
    [Binding]
    public class CallbacksSteps : StepsBase
    {
        private readonly CallbackEndpoints _endpoints = new ApiUriFactory().CallbackEndpoints;

        public CallbacksSteps(ApiTestContext apiTestContext) : base(apiTestContext)
        {
        }

        [Given(@"I have a valid conference event request for event type (.*)")]
        public async Task GivenIHaveAnConferenceEventRequestForAnEventType(EventType eventType)
        {
            var seededConference = await ApiTestContext.TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            ApiTestContext.NewConferenceId = seededConference.Id;
            var request = BuildRequest(eventType, seededConference);
            ApiTestContext.Uri = _endpoints.Event;
            ApiTestContext.HttpMethod = HttpMethod.Post;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            ApiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have a (.*) conference event request")]
        [Given(@"I have an (.*) conference event request")]
        public void GivenIHaveAnConferenceEventRequest(Scenario scenario)
        {
            ConferenceEventRequest request;
            switch (scenario)
            {
                case Scenario.Nonexistent:
                    request = BuildRequest(EventType.Transfer);
                    break;
                case Scenario.Invalid:
                    request = BuildInvalidRequest();
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            ApiTestContext.Uri = _endpoints.Event;
            ApiTestContext.HttpMethod = HttpMethod.Post;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            ApiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have a room transfer event request for a nonexistent participant")]
        public async Task GivenIRoomTransferEventRequestForNonExistentParticipant()
        {
            var seededConference = await ApiTestContext.TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            ApiTestContext.NewConferenceId = seededConference.Id;
            var request = BuildRequest(EventType.Transfer, seededConference);
            request.ParticipantId = Guid.NewGuid().ToString();

            ApiTestContext.Uri = _endpoints.Event;
            ApiTestContext.HttpMethod = HttpMethod.Post;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            ApiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
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
            request.ParticipantId = conference.GetParticipants().First().Id.ToString();
            return request;
        }

        private ConferenceEventRequest BuildInvalidRequest()
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