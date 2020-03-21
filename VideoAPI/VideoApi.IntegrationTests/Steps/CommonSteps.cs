using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain.Enums;
using VideoApi.IntegrationTests.Contexts;

namespace VideoApi.IntegrationTests.Steps
{
    [Binding]
    public sealed class CommonSteps : BaseSteps
    {
        private readonly TestContext _context;

        public CommonSteps(TestContext c)
        {
            _context = c;
        }

        [Given(@"I have a conference")]
        public async Task GivenIHaveAConference()
        {
            _context.Test.Conference = await _context.TestDataManager.SeedConference();
            NUnit.Framework.TestContext.WriteLine($"New seeded conference id: {_context.Test.Conference.Id}");
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
            _context.Test.Conference = await _context.TestDataManager.SeedConference(conference);
        }
        
        [Given(@"I have several conferences")]
        public async Task GivenIHaveManyConferences()
        {
            var today = DateTime.Today.ToUniversalTime().AddMinutes(1);
            var tomorrow = DateTime.Today.ToUniversalTime().AddDays(1).AddMinutes(1);
            var yesterday = DateTime.Today.ToUniversalTime().AddDays(-1).AddMinutes(1);
            
            var yesterdayClosedConference = new ConferenceBuilder(true, scheduledDateTime: yesterday)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .WithParticipantTask("Disconnected")
                .Build();

            var todayConference1 = new ConferenceBuilder(true, scheduledDateTime: today)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.InSession)
                .WithParticipantTask("Disconnected")
                .Build();

            var tomorrowConference1 = new ConferenceBuilder(true, scheduledDateTime: tomorrow)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Paused)
                .WithParticipantTask("Disconnected")
                .Build();

            var todayConference2 = new ConferenceBuilder(true, scheduledDateTime: today)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Suspended)
                .WithParticipantTask("Disconnected")
                .Build();

            var tomorrowConference2 = new ConferenceBuilder(true, scheduledDateTime: tomorrow)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Suspended)
                .WithParticipantTask("Disconnected")
                .Build();

            var yesterdayConference2 = new ConferenceBuilder(true, scheduledDateTime: yesterday)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithParticipantTask("Disconnected")
                .Build();
            
            _context.Test.ClosedConferences.Add(await _context.TestDataManager.SeedConference(yesterdayClosedConference));
            _context.Test.YesterdayClosedConference = _context.Test.ClosedConferences.First();
            _context.Test.TodaysConferences.Add(await _context.TestDataManager.SeedConference(todayConference1));
            await _context.TestDataManager.SeedConference(tomorrowConference1);
            _context.Test.TodaysConferences.Add(await _context.TestDataManager.SeedConference(todayConference2));
            await _context.TestDataManager.SeedConference(tomorrowConference2);
            _context.Test.ClosedConferences.Add(await _context.TestDataManager.SeedConference(yesterdayConference2));

            _context.Test.Conferences.Add(yesterdayClosedConference);
            _context.Test.Conferences.Add(todayConference1);
            _context.Test.Conferences.Add(tomorrowConference1);
            _context.Test.Conferences.Add(todayConference2);
            _context.Test.Conferences.Add(tomorrowConference2);
            _context.Test.Conferences.Add(yesterdayConference2);
        }

        [Given(@"I have several closed conferences with messages")]
        public async Task GivenIHaveSeveralClosedConferencesWithMessages()
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
            conferenceType.GetProperty("ClosedDateTime").SetValue(conference1, DateTime.UtcNow.AddMinutes(-20));
            conferenceList.Add(conference1);

            var conference2 = new ConferenceBuilder(true, scheduledDateTime: oldHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .WithParticipantTask("Disconnected")
                .Build();
            conferenceType.GetProperty("ClosedDateTime").SetValue(conference1, DateTime.UtcNow.AddMinutes(-40));
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
            conferenceType.GetProperty("ClosedDateTime").SetValue(conference4, DateTime.UtcNow.AddMinutes(-20));
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
                if (c.ClosedDateTime!= null &&
                    c.ClosedDateTime.Value.ToUniversalTime() < DateTime.Now.ToUniversalTime().AddMinutes(-30) &&
                    c.InstantMessageHistory.Count > 0)
                {
                    _context.Test.ClosedConferencesWithMessages.Add(c);
                }
                _context.Test.Conferences.Add(await _context.TestDataManager.SeedConference(c));
            }
        }

        [Given(@"I have a many very old closed conferences with messages")]
        public async Task GivenIHaveAManyVeryOldClosedConferencesWithMessages()
        {
            var conferenceList = new List<Domain.Conference>();
            var conferenceType = typeof(Domain.Conference);
            var utcDate = DateTime.UtcNow;
            var oldHearing = utcDate.AddMonths(-4);

            var conference1 = new ConferenceBuilder(true, scheduledDateTime: oldHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .WithParticipantTask("Disconnected")
                .WithMessages(3)
                .Build();
            conferenceType.GetProperty("ClosedDateTime").SetValue(conference1, DateTime.UtcNow.AddMonths(-3));
            conferenceList.Add(conference1);

            var conference2 = new ConferenceBuilder(true, scheduledDateTime: oldHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .WithParticipantTask("Disconnected")
                .Build();
            conferenceType.GetProperty("ClosedDateTime").SetValue(conference1, DateTime.UtcNow.AddMonths(-2));
            conferenceList.Add(conference2);

            var conference3 = new ConferenceBuilder(true, scheduledDateTime: oldHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .WithParticipantTask("Disconnected")
                .WithMessages(3)
                .Build();
            conferenceType.GetProperty("ClosedDateTime").SetValue(conference3, DateTime.UtcNow.AddMonths(-1));
            conferenceList.Add(conference3);

            foreach (var c in conferenceList)
            {
                _context.Test.Conferences.Add(await _context.TestDataManager.SeedConference(c));
            }
        }

        [Given(@"I have a many closed conferences with no messages")]
        public async Task GivenIHaveAManyClosedConferencesWithNoMessages()
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
                .WithConferenceStatus(ConferenceState.Closed)
                .WithParticipantTask("Disconnected")
                .Build();
            conferenceType.GetProperty("ClosedDateTime").SetValue(conference3, DateTime.UtcNow.AddMinutes(-29));
            conferenceList.Add(conference3);

            foreach (var c in conferenceList)
            {
                _context.Test.Conferences.Add(await _context.TestDataManager.SeedConference(c));
            }
        }

        [Given(@"I have a many open conferences with messages")]
        public async Task GivenIHaveAManyOpenConferencesWithMessages()
        {
            var conferenceList = new List<Domain.Conference>();
            var conferenceType = typeof(Domain.Conference);
            var utcDate = DateTime.UtcNow;
            var oldHearing = utcDate.AddMinutes(-180);

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
                .WithConferenceStatus(ConferenceState.InSession)
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
                .WithConferenceStatus(ConferenceState.Suspended)
                .WithParticipantTask("Disconnected")
                .Build();
            conferenceList.Add(conference6);

            foreach (var c in conferenceList)
            {
                _context.Test.Conferences.Add(await _context.TestDataManager.SeedConference(c));
            }
        }

        [When(@"I send the request to the endpoint")]
        [When(@"I send the same request twice")]
        public async Task WhenISendTheRequestToTheEndpoint()
        {
            _context.Response = _context.HttpMethod.Method switch
            {
                "GET" => await SendGetRequestAsync(_context),
                "POST" => await SendPostRequestAsync(_context),
                "PATCH" => await SendPatchRequestAsync(_context),
                "PUT" => await SendPutRequestAsync(_context),
                "DELETE" => await SendDeleteRequestAsync(_context),
                _ => throw new ArgumentOutOfRangeException(_context.HttpMethod.ToString(),
                    _context.HttpMethod.ToString(), null)
            };
        }

        [Then(@"the response should have the status (.*) and success status (.*)")]
        public void ThenTheResponseShouldHaveStatus(HttpStatusCode statusCode, bool isSuccess)
        {
            _context.Response.StatusCode.Should().Be(statusCode);
            _context.Response.IsSuccessStatusCode.Should().Be(isSuccess);
            Zap.Scan($"{_context.Config.VhServices.VideoApiUrl}{_context.Uri}");
            NUnit.Framework.TestContext.WriteLine($"Status Code: {_context.Response.StatusCode}");
        }

        [Then(@"the response message should read '(.*)'")]
        [Then(@"the error response message should contain '(.*)'")]
        [Then(@"the error response message should also contain '(.*)'")]
        public async Task ThenTheResponseShouldContain(string errorMessage)
        {
            var messageString = await _context.Response.Content.ReadAsStringAsync();
            messageString.Should().Contain(errorMessage);
            Zap.Scan($"{_context.Config.VhServices.VideoApiUrl}{_context.Uri}");
        }
    }
}
