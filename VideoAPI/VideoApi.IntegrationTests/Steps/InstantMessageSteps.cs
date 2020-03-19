using Faker;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using TechTalk.SpecFlow;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Common.Helpers;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.IntegrationTests.Contexts;
using VideoApi.IntegrationTests.Helper;
using Task = System.Threading.Tasks.Task;
using static Testing.Common.Helper.ApiUriFactory.InstantMessageEndpoints;

namespace VideoApi.IntegrationTests.Steps
{
    [Binding]
    public class InstantMessageSteps : BaseSteps
    {
        private readonly TestContext _context;
        public InstantMessageSteps(TestContext context)
        {
            _context = context;
        }

        [Given(@"I have a (.*) conference with messages")]
        [Given(@"I have an (.*) conference with messages")]
        public void GivenIHaveAConferenceWithMessages(Scenario scenario)
        {
            var conferenceId = _context.Test.Conference.Id;
            var judge = _context.Test.Conference.Participants.First(x => x.IsJudge());
            _context.Test.Conference.AddInstantMessage(judge.DisplayName, "test message from Judge");
            _context.Test.Conference.AddInstantMessage("VH Officer ", "test message from VHO");
            switch (scenario)
            {
                case Scenario.Valid:
                    break;
                case Scenario.Nonexistent:
                    conferenceId = Guid.NewGuid();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            _context.Uri = GetInstantMessageHistory(conferenceId);
            _context.HttpMethod = HttpMethod.Get;
        }

        [Then(@"the chat messages should be retrieved")]
        public async Task ThenTheChatMessagesShouldBeRetrieved()
        {
            var json = await _context.ResponseMessage.Content.ReadAsStringAsync();
            var messages = RequestHelper.DeserialiseSnakeCaseJsonToResponse<List<InstantMessageResponse>>(json);
            messages.Should().NotBeNullOrEmpty();
            messages.Should().BeInDescendingOrder(x => x.TimeStamp);
            foreach (var message in messages)
            {
                message.From.Should().NotBeNullOrWhiteSpace();
                message.MessageText.Should().NotBeNullOrWhiteSpace();
            }
        }

        [Given(@"I have a (.*) conference with (.*) participants save message request")]
        [Given(@"I have an (.*) conference with (.*) participants save message request")]
        public void GivenIHaveASaveMessageRequest(Scenario conferenceScenario,
            Scenario participantScenario)
        {
            Guid conferenceId;
            var from = string.Empty;
            switch (conferenceScenario)
            {
                case Scenario.Valid:
                    if (participantScenario == Scenario.Valid)
                    {
                        from = _context.Test.Conference.Participants.First(x => x.UserRole == UserRole.Judge).DisplayName;
                    }

                    conferenceId = _context.Test.Conference.Id;
                    break;
                case Scenario.Nonexistent:
                    conferenceId = Guid.NewGuid();
                    from = "non-existentuser";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(conferenceScenario), conferenceScenario, null);
            }

            _context.Uri = SaveInstantMessage(conferenceId);
            _context.HttpMethod = HttpMethod.Post;
            var request = new AddInstantMessageRequest
            {
                From = from,
                MessageText = Internet.DomainWord()
            };
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I send the request to the get closed conferences endpoint")]
        public void GivenISendTheRequestToTheGetClosedConferencesEndpoint()
        {
            _context.Uri = GetClosedConferencesWithInstantMessages;
            _context.HttpMethod = HttpMethod.Get;
        }

        [Then(@"the responses list should contain closed conferences")]
        public async Task ThenTheResponsesListShouldContainClosedConferences()
        {
            var conferences = await GetResponses<List<ClosedConferencesResponse>>();
            conferences.Should().NotBeEmpty();
        }

        [Then(@"the response is an empty list should")]
        public async Task ThenTheResponseIsAnEmptyList()
        {
            var conferences = await GetResponses<List<ClosedConferencesResponse>>();
            conferences.Should().BeEmpty();
        }

        [Given(@"I have a remove messages from a (.*) conference request")]
        [Given(@"I have a remove messages from an (.*) conference request")]
        public void GivenIHaveAnRemoveMessagesFromAValidConferenceRequestAsync(Scenario scenario)
        {
            var conferenceId = _context.Test.Conference.Id; ;
            switch (scenario)
            {
                case Scenario.Valid:
                    {
                        break;
                    }
                case Scenario.Nonexistent:
                    conferenceId = Guid.NewGuid();
                    break;
                case Scenario.Invalid:
                    conferenceId = Guid.Empty;
                    break;

                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            _context.Uri = RemoveInstantMessagesForConference(conferenceId);
            _context.HttpMethod = HttpMethod.Delete;
        }

        private async Task<T> GetResponses<T>()
        {
            var json = await _context.ResponseMessage.Content.ReadAsStringAsync();
            return ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<T>(json);
        }
    }
}
