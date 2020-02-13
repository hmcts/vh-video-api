using Faker;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Common.Helpers;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.IntegrationTests.Contexts;
using VideoApi.IntegrationTests.Helper;

namespace VideoApi.IntegrationTests.Steps
{
    [Binding]
    public class MessageSteps : StepsBase
    {
        private readonly MessageEndpoints _endpoints = new ApiUriFactory().MessageEndpoints;

        public MessageSteps(ApiTestContext apiTestContext) : base(apiTestContext)
        {
        }

        [Given(@"I have a (.*) conference with messages")]
        [Given(@"I have an (.*) conference with messages")]
        public async System.Threading.Tasks.Task GivenIHaveAConferenceWithMessages(Scenario scenario)
        {
            Guid conferenceId;
            switch (scenario)
            {
                case Scenario.Valid:
                    var seededConference = await SeedConferenceWithMessages();
                    TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
                    ApiTestContext.NewConferenceId = seededConference.Id;
                    conferenceId = seededConference.Id;
                    break;
                case Scenario.Nonexistent:
                    conferenceId = Guid.NewGuid();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            ApiTestContext.Uri = _endpoints.GetMessages(conferenceId);
            ApiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Then(@"the chat messages should be retrieved")]
        public async System.Threading.Tasks.Task ThenTheChatMessagesShouldBeRetrieved()
        {
            var json = await ApiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var messages = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<MessageResponse>>(json);
            messages.Should().NotBeNullOrEmpty();
            messages.Should().BeInDescendingOrder(x => x.TimeStamp);
            foreach (var message in messages)
            {
                message.From.Should().NotBeNullOrWhiteSpace();
                message.To.Should().NotBeNullOrWhiteSpace();
                message.MessageText.Should().NotBeNullOrWhiteSpace();
            }
        }

        [Given(@"I have a (.*) conference with (.*) participants save message request")]
        [Given(@"I have an (.*) conference with (.*) participants save message request")]
        public async System.Threading.Tasks.Task GivenIHaveASaveMessageRequest(Scenario conferenceScenario, Scenario particpiantScenario)
        {
            Guid conferenceId;
            string from = Internet.Email();
            string to = Internet.Email();
            switch (conferenceScenario)
            {
                case Scenario.Valid:
                    var seededConference = await ApiTestContext.TestDataManager.SeedConference();
                    var judge = seededConference.GetParticipants().First(x => x.UserRole == UserRole.Judge);
                    if(particpiantScenario == Scenario.Valid)
                    {
                        var particpiants = seededConference.Participants;
                        from = particpiants.First(x => x.UserRole == UserRole.Judge).Username;
                        to = particpiants.First(x => x.UserRole == UserRole.Individual).Username;
                    }
                   
                    TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
                    ApiTestContext.NewConferenceId = seededConference.Id;
                    conferenceId = seededConference.Id;
                    break;
                case Scenario.Nonexistent:
                    conferenceId = Guid.NewGuid();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(conferenceScenario), conferenceScenario, null);
            }

            ApiTestContext.Uri = _endpoints.SaveMessage(conferenceId);
            ApiTestContext.HttpMethod = HttpMethod.Post;
            var request = new AddMessageRequest
            {
                From = from,
                To = to,
                MessageText = Internet.DomainWord()
            };
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            ApiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }


        private async Task<Conference> SeedConferenceWithMessages()
        {
            var conference = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Individual, "Claimant")
                .WithParticipant(UserRole.Judge, "Judge")
                .Build();

            var judge = conference.GetParticipants().First(x => x.UserRole == UserRole.Judge);
            var participantUsername = Internet.Email();
            conference.AddMessage(judge.Username, participantUsername, Internet.DomainWord());
            conference.AddMessage(participantUsername, judge.Username, Internet.DomainWord());
            return await ApiTestContext.TestDataManager.SeedConference(conference);
        }
    }
}
