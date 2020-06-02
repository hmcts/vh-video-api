using Faker;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using AcceptanceTests.Common.Api.Helpers;
using TechTalk.SpecFlow;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
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
        private readonly CommonSteps _commonSteps;
        public InstantMessageSteps(TestContext context, CommonSteps commonSteps)
        {
            _context = context;
            _commonSteps = commonSteps;
        }

        [Given(@"the conference has messages")]
        public async Task GivenTheConferenceHasMessages()
        {
            GivenIHaveASaveMessageRequest(Scenario.Valid);
            await _commonSteps.WhenISendTheRequestToTheEndpoint();
            _context.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Given(@"I have a (.*) get instant messages request")]
        [Given(@"I have an (.*) get instant messages request")]
        public void GivenIHaveAConferenceWithMessages(Scenario scenario)
        {
            var conferenceId = scenario == Scenario.Nonexistent ? Guid.NewGuid() : _context.Test.Conference.Id;
            _context.Uri = GetInstantMessageHistory(conferenceId);
            _context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a (.*) set instant message request")]
        [Given(@"I have an (.*) set instant message request")]
        public void GivenIHaveASaveMessageRequest(Scenario scenario)
        {
            Guid conferenceId;
            string from;
            string to;
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    from = _context.Test.Conference.Participants.First(x => x.UserRole == UserRole.Judge).DisplayName;
                    conferenceId = _context.Test.Conference.Id;
                    to = "VH Officer";
                    break;
                }
                case Scenario.Nonexistent:
                    conferenceId = Guid.NewGuid();
                    from = "non-existent-user";
                    to = "non-existant-receiver";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            _context.Uri = SaveInstantMessage(conferenceId);
            _context.HttpMethod = HttpMethod.Post;
            _context.Test.Message = new AddInstantMessageRequest
            {
                From = from,
                MessageText = Internet.DomainWord(),
                To = to
            };
            var jsonBody = RequestHelper.SerialiseRequestToSnakeCaseJson(_context.Test.Message);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

       [Given(@"I have a valid set instant message request with a nonexistent participant")]
        public void GivenIHaveAValidSetInstantMessageRequestWithANonexistentParticipant()
        {
            _context.Uri = SaveInstantMessage(_context.Test.Conference.Id);
            _context.HttpMethod = HttpMethod.Post;
            var request = new AddInstantMessageRequest
            {
                From = "non-existent-participant",
                MessageText = Internet.DomainWord()
            };
            var jsonBody = RequestHelper.SerialiseRequestToSnakeCaseJson(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have a (.*) delete messages from a conference request")]
        [Given(@"I have an (.*) delete messages from a conference request")]
        public void GivenIHaveAnRemoveMessagesFromAValidConferenceRequestAsync(Scenario scenario)
        {
            var conferenceId = scenario switch
            {
                Scenario.Valid => _context.Test.Conference.Id,
                Scenario.Nonexistent => Guid.NewGuid(),
                Scenario.Invalid => Guid.Empty,
                _ => throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null)
            };
            _context.Uri = RemoveInstantMessagesForConference(conferenceId);
            _context.HttpMethod = HttpMethod.Delete;
        }

        [Given(@"I have a valid get closed conferences with messages request")]
        public void GivenIHaveAValidGetClosedConferencesWithMessagesRequest()
        {
            _context.Uri = GetClosedConferencesWithInstantMessages;
            _context.HttpMethod = HttpMethod.Get;
        }

        [Then(@"the responses list should contain closed conferences")]
        public async Task ThenTheResponsesListShouldContainClosedConferences()
        {
            var conferences = await Response.GetResponses<List<ClosedConferencesResponse>>(_context.Response.Content);
            foreach (var conference in conferences)
            {
                _context.Test.ClosedConferencesWithMessages.Any(x => x.Id.Equals(conference.Id)).Should().BeTrue();
            }
        }

        [Then(@"the response returns an empty list without messages")]
        public async Task ThenTheResponseReturnsAnEmptyListWithoutMessages()
        {
            var conferences = await Response.GetResponses<List<ClosedConferencesResponse>>(_context.Response.Content);
            conferences.Count.Should().Be(0);
        }

        [Then(@"the messages have been deleted")]
        public async Task ThenTheMessagesHaveBeenDeleted()
        {
            _context.Uri = GetInstantMessageHistory(_context.Test.Conference.Id);
            _context.HttpMethod = HttpMethod.Get;
            await _commonSteps.WhenISendTheRequestToTheEndpoint();
            _context.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            var response = await Response.GetResponses<List<InstantMessageResponse>>(_context.Response.Content);
            response.Count.Should().Be(0);
        }

        [Then(@"the chat message should be retrieved")]
        public async Task ThenTheChatMessagesShouldBeRetrieved()
        {
            var messages = await Response.GetResponses<List<InstantMessageResponse>>(_context.Response.Content);
            messages.Should().NotBeNullOrEmpty();
            messages.Should().BeInDescendingOrder(x => x.TimeStamp);
            foreach (var message in messages)
            {
                message.From.Should().Be(_context.Test.Message.From);
                message.MessageText.Should().Be(_context.Test.Message.MessageText);
                message.To.Should().Be(_context.Test.Message.To);
            }
        }
    }
}
