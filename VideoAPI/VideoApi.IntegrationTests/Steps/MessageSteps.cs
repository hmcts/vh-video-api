using Faker;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Common.Helpers;
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

        //public async Task<Conference> SeedConferenceWithMessages(int count)
        //{
        //    var conference = await SeedConference();
        //    for (int i = 0; i < count; i++)
        //    {
        //        conference.AddMessage(Internet.Email(), Internet.Email(), $"Message {i + 1}");
        //    }

        //    return await SeedConference(conference);
        //}
    }
}
