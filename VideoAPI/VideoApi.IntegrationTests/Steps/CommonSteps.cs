using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain.Enums;
using VideoApi.IntegrationTests.Contexts;

namespace VideoApi.IntegrationTests.Steps
{
    [Binding]
    public sealed class CommonSteps : StepsBase
    {
        private readonly ConferenceTestContext _conferenceTestContext;

        public CommonSteps(ApiTestContext apiTestContext, ConferenceTestContext conferenceTestContext) : base(
            apiTestContext)
        {
            _conferenceTestContext = conferenceTestContext;
        }

        [Given(@"I have a conference")]
        public async Task GivenIHaveAConference()
        {
            var seededConference = await ApiTestContext.TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            ApiTestContext.NewConferenceId = seededConference.Id;
            _conferenceTestContext.SeededConference = seededConference;
        }
        
        [Given(@"I have a conference today")]
        public async Task GivenIHaveAConferenceToday()
        {
            var conference = new ConferenceBuilder(true, null, DateTime.UtcNow.AddMinutes(5))
                .WithParticipant(UserRole.Individual, "Claimant")
                .WithParticipant(UserRole.Representative, "Claimant")
                .WithParticipant(UserRole.Individual, "Defendant")
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.InSession)
                .WithHearingTask("Suspended")
                .Build();
            var seededConference = await ApiTestContext.TestDataManager.SeedConference(conference);
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            ApiTestContext.NewConferenceId = seededConference.Id;
            _conferenceTestContext.SeededConference = seededConference;
        }
        
        [Given(@"I have a many conferences")]
        public async Task GivenIHaveManyConferences()
        {
            var today = DateTime.Today.AddHours(10);
            var tomorrow = DateTime.Today.AddDays(1).AddHours(10);
            var yesterday = DateTime.Today.AddDays(-1).AddHours(10);
            
            var conference1 = new ConferenceBuilder(true, scheduledDateTime: yesterday)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .WithParticipantTask("Disconnected")
                .Build();

            var conference2 = new ConferenceBuilder(true, scheduledDateTime: today)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.InSession)
                .WithParticipantTask("Disconnected")
                .Build();

            var conference3 = new ConferenceBuilder(true, scheduledDateTime: tomorrow)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Paused)
                .WithParticipantTask("Disconnected")
                .Build();

            var conference4 = new ConferenceBuilder(true, scheduledDateTime: today)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Suspended)
                .WithParticipantTask("Disconnected")
                .Build();

            var conference5 = new ConferenceBuilder(true, scheduledDateTime: tomorrow)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Suspended)
                .WithParticipantTask("Disconnected")
                .Build();

            var conference6 = new ConferenceBuilder(true, scheduledDateTime: yesterday)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithParticipantTask("Disconnected")
                .Build();
            
            await ApiTestContext.TestDataManager.SeedConference(conference1);
            await ApiTestContext.TestDataManager.SeedConference(conference2);
            await ApiTestContext.TestDataManager.SeedConference(conference3);
            await ApiTestContext.TestDataManager.SeedConference(conference4);
            await ApiTestContext.TestDataManager.SeedConference(conference5);
            await ApiTestContext.TestDataManager.SeedConference(conference6);
            
            _conferenceTestContext.SeededConferences.Add(conference1.Id);
            _conferenceTestContext.SeededConferences.Add(conference2.Id);
            _conferenceTestContext.SeededConferences.Add(conference3.Id);
            _conferenceTestContext.SeededConferences.Add(conference4.Id);
            _conferenceTestContext.SeededConferences.Add(conference5.Id);
            _conferenceTestContext.SeededConferences.Add(conference6.Id);
        }

        [Given(@"I have a many closed conferences with messages")]
        public async Task GivenIHaveAManyClosedConferencesWithMessages()
        {
            var conferenceList = new List<Domain.Conference>();
            var conferenceType = typeof(Domain.Conference);
            var utcDate = DateTime.UtcNow;
            var currentHearing = utcDate.AddMinutes(-40);
            var oldHearing = utcDate.AddMinutes(-180);

            var conference1 = new ConferenceBuilder(true, scheduledDateTime: currentHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .WithParticipantTask("Disconnected")
                .WithMessages(3)
                .Build();
            conferenceType.GetProperty("ClosedDateTime").SetValue(conference1, DateTime.UtcNow.AddMinutes(-30));
            conferenceList.Add(conference1);

            var conference2 = new ConferenceBuilder(true, scheduledDateTime: oldHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .WithParticipantTask("Disconnected")
                .Build();
            conferenceType.GetProperty("ClosedDateTime").SetValue(conference1, DateTime.UtcNow.AddMinutes(-31));
            conferenceList.Add(conference2);

            var conference3 = new ConferenceBuilder(true, scheduledDateTime: oldHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Paused)
                .WithParticipantTask("Disconnected")
                .WithMessages(3)
                .Build();
            conferenceList.Add(conference3);

            var conference4 = new ConferenceBuilder(true, scheduledDateTime: oldHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .WithParticipantTask("Disconnected")
                .WithMessages(3)
                .Build();
            conferenceType.GetProperty("ClosedDateTime").SetValue(conference4, DateTime.UtcNow.AddMinutes(-30));
            conferenceList.Add(conference4);

            var conference5 = new ConferenceBuilder(true, scheduledDateTime: oldHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.InSession)
                .WithParticipantTask("Disconnected")
                .WithMessages(3)
                .Build();
            conferenceList.Add(conference5);

            var conference6 = new ConferenceBuilder(true, scheduledDateTime: oldHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithParticipantTask("Disconnected")
                .Build();
            conferenceList.Add(conference6);

            foreach(var c in conferenceList)
            {
                await ApiTestContext.TestDataManager.SeedConference(c);
            }

            foreach (var c in conferenceList)
            {
                _conferenceTestContext.SeededConferences.Add(c.Id);
            }
        }

        [When(@"I send the request to the endpoint")]
        [When(@"I send the same request twice")]
        public async Task WhenISendTheRequestToTheEndpoint()
        {
            ApiTestContext.ResponseMessage = new HttpResponseMessage();
            switch (ApiTestContext.HttpMethod.Method)
            {
                case "GET":
                    ApiTestContext.ResponseMessage = await SendGetRequestAsync(ApiTestContext);
                    break;
                case "POST":
                    ApiTestContext.ResponseMessage = await SendPostRequestAsync(ApiTestContext);
                    break;
                case "PATCH":
                    ApiTestContext.ResponseMessage = await SendPatchRequestAsync(ApiTestContext);
                    break;
                case "PUT":
                    ApiTestContext.ResponseMessage = await SendPutRequestAsync(ApiTestContext);
                    break;
                case "DELETE":
                    ApiTestContext.ResponseMessage = await SendDeleteRequestAsync(ApiTestContext);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(ApiTestContext.HttpMethod.ToString(),
                        ApiTestContext.HttpMethod.ToString(), null);
            }
        }

        [Then(@"the response should have the status (.*) and success status (.*)")]
        public void ThenTheResponseShouldHaveStatus(HttpStatusCode statusCode, bool isSuccess)
        {
            ApiTestContext.ResponseMessage.StatusCode.Should().Be(statusCode);
            ApiTestContext.ResponseMessage.IsSuccessStatusCode.Should().Be(isSuccess);
            ZAP.Scan(ApiTestContext.RequestUrl);
            TestContext.WriteLine($"Status Code: {ApiTestContext.ResponseMessage.StatusCode}");
        }

        [Then(@"the response message should read '(.*)'")]
        [Then(@"the error response message should contain '(.*)'")]
        [Then(@"the error response message should also contain '(.*)'")]
        public async Task ThenTheResponseShouldContain(string errorMessage)
        {
            var messageString = await ApiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            messageString.Should().Contain(errorMessage);
            ZAP.Scan(ApiTestContext.RequestUrl);
        }
    }
}
