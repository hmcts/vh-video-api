using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TechTalk.SpecFlow;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.IntegrationTests.Contexts;
using Alert = VideoApi.Domain.Task;
using Task = System.Threading.Tasks.Task;

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
                .Build();

            _context.Test.Conference = await _context.TestDataManager.SeedConference(conference);
            _context.Test.Alerts = await _context.TestDataManager.SeedAlerts(new[]
                {new Alert(conference.Id, conference.Id, "Suspended", TaskType.Hearing)});
        }

        [Given(@"I have several conferences")]
        public async Task GivenIHaveManyConferences()
        {
            var today = DateTime.Today.ToUniversalTime().AddMinutes(1);
            var tomorrow = DateTime.Today.ToUniversalTime().AddDays(1).AddMinutes(1);
            var yesterday = DateTime.Today.ToUniversalTime().AddDays(-1).AddMinutes(1);

            var venue1 = "Manchester";
            var venue2 = "Birmingham";

            var yesterdayClosedConference = new ConferenceBuilder(true, scheduledDateTime: yesterday, venueName: venue1)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .Build();

            var todayConference1 = new ConferenceBuilder(true, scheduledDateTime: today, venueName: venue1)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.InSession)
                .Build();

            var tomorrowConference1 = new ConferenceBuilder(true, scheduledDateTime: tomorrow, venueName: venue1)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Paused)
                .Build();

            var todayConference2 = new ConferenceBuilder(true, scheduledDateTime: today, venueName: venue2)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Suspended)
                .Build();

            var tomorrowConference2 = new ConferenceBuilder(true, scheduledDateTime: tomorrow, venueName: venue2)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Suspended)
                .Build();

            var yesterdayConference2 = new ConferenceBuilder(true, scheduledDateTime: yesterday, venueName: venue2)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .Build();

            _context.Test.ClosedConferences.Add(
                await _context.TestDataManager.SeedConference(yesterdayClosedConference));
            _context.Test.YesterdayClosedConference = _context.Test.ClosedConferences.First();
            _context.Test.TodaysConferences.Add(await _context.TestDataManager.SeedConference(todayConference1));
            await _context.TestDataManager.SeedConference(tomorrowConference1);
            _context.Test.TodaysConferences.Add(await _context.TestDataManager.SeedConference(todayConference2));
            await _context.TestDataManager.SeedConference(tomorrowConference2);
            _context.Test.ClosedConferences.Add(await _context.TestDataManager.SeedConference(yesterdayConference2));

            var alert1 = new Alert(yesterdayClosedConference.Id,yesterdayClosedConference.Id, "Disconnected", TaskType.Participant);
            var alert2 = new Alert(todayConference1.Id,todayConference1.Id, "Disconnected", TaskType.Participant);
            var alert3 = new Alert(tomorrowConference1.Id,tomorrowConference1.Id, "Disconnected", TaskType.Participant);
            var alert4 = new Alert(todayConference2.Id,todayConference2.Id, "Disconnected", TaskType.Participant);
            var alert5 = new Alert(todayConference2.Id,todayConference2.Id, "Disconnected", TaskType.Participant);
            var alert6 = new Alert(yesterdayConference2.Id,yesterdayConference2.Id, "Disconnected", TaskType.Participant);

            _context.Test.Conferences.Add(yesterdayClosedConference);
            _context.Test.Conferences.Add(todayConference1);
            _context.Test.Conferences.Add(tomorrowConference1);
            _context.Test.Conferences.Add(todayConference2);
            _context.Test.Conferences.Add(tomorrowConference2);
            _context.Test.Conferences.Add(yesterdayConference2);

            _context.Test.Alerts = await _context.TestDataManager.SeedAlerts(new List<Alert>
            {
                alert1, alert2, alert3, alert4, alert5, alert6
            });
        }

        [Given(@"I have several conferences with users")]
        public async Task GivenIHaveSeveralConferencesWithUsers()
        {
            var today = DateTime.Today.AddMinutes(10);
            var conference1 = new ConferenceBuilder(true, scheduledDateTime: today)
                .WithParticipants(new List<Participant>
                {
                    new Participant(Guid.NewGuid(),"", "JudgeOne", "Smith", "JudgeOne Smith", "JudgeOne.Smith@email.com", UserRole.Judge, "ChildrenAct"),
                    new Participant(Guid.NewGuid(),"", "IndividualOne", "Brown", "IndividualOne Brown", "IndividualOne.Smith@email.com", UserRole.Individual, "ChildrenAct")
                })
                .WithMeetingRoom("https://poc.node.com", "user@email.com")
                .Build();
            today = DateTime.Today.AddMinutes(20);
            var conference2 = new ConferenceBuilder(true, scheduledDateTime: today)
                .WithParticipants(new List<Participant>
                {
                    new Participant(Guid.NewGuid(),"", "JudgeOne", "Smith", "JudgeOne Smith", "JudgeOne.Smith@email.com", UserRole.Judge, "ChildrenAct"),
                    new Participant(Guid.NewGuid(),"", "RepresentativeOne", "Green", "RepresentativeOne Green", "RepresentativeOne.Green@email.com", UserRole.Individual, "ChildrenAct")
                })
                .WithMeetingRoom("https://poc.node.com", "user@email.com")
                .Build();
            today = DateTime.Today.AddMinutes(30);
            var conference3 = new ConferenceBuilder(true, scheduledDateTime: today)
                .WithParticipants(new List<Participant>
                {
                    new Participant(Guid.NewGuid(),"", "JudgeOne", "Smith", "JudgeOne Smith", "JudgeOne.Smith@email.com", UserRole.Judge, "ChildrenAct"),
                    new Participant(Guid.NewGuid(),"", "RepresentativeTwo", "Brown", "RepresentativeTwo Brown", "RepresentativeTwo.Brown@email.com", UserRole.Individual, "ChildrenAct")
                })
                .WithMeetingRoom("https://poc.node.com", "user@email.com")
                .Build();
            today = DateTime.Today.AddMinutes(35);
            var conference4 = new ConferenceBuilder(true, scheduledDateTime: today)
                .WithParticipants(new List<Participant>
                {
                    new Participant(Guid.NewGuid(),"", "JudgeTwo", "Dave", "JudgeTwo Dave", "JudgeTwo.Dave@email.com", UserRole.Judge, "ChildrenAct"),
                    new Participant(Guid.NewGuid(),"", "RepresentativeOne", "Green", "RepresentativeOne Green", "RepresentativeOne.Green@email.com", UserRole.Individual, "ChildrenAct")
                })
                .WithMeetingRoom("https://poc.node.com", "user@email.com")
                .Build();
            today = DateTime.Today.AddMinutes(40);
            var conference5 = new ConferenceBuilder(true, scheduledDateTime: today)
                .WithParticipants(new List<Participant>
                {
                    new Participant(Guid.NewGuid(),"", "JudgeFour", "Matt", "JudgeFour Matt", "JudgeFour.Matt@email.com", UserRole.Judge, "ChildrenAct"),
                    new Participant(Guid.NewGuid(),"", "RepresentativeTwo", "Dredd", "RepresentativeTwo Dredd", "RepresentativeTwo.Dredd@email.com", UserRole.Individual, "ChildrenAct")
                })
                .WithMeetingRoom("https://poc.node.com", "user@email.com")
                .Build();
            today = DateTime.Today.AddMinutes(45);
            var conference6 = new ConferenceBuilder(true, scheduledDateTime: today)
                .WithParticipants(new List<Participant>
                {
                    new Participant(Guid.NewGuid(),"", "JudgeFour", "Matt", "JudgeFour Matt", "JudgeFour.Matt@email.com", UserRole.Judge, "ChildrenAct"),
                    new Participant(Guid.NewGuid(),"", "IndividualOne", "Brown", "IndividualOne Brown", "IndividualOne.Smith@email.com", UserRole.Individual, "ChildrenAct")
                })
                .WithMeetingRoom("https://poc.node.com", "user@email.com")
                .Build();
            await _context.TestDataManager.SeedConference(conference1);
            await _context.TestDataManager.SeedConference(conference2);
            await _context.TestDataManager.SeedConference(conference3);
            await _context.TestDataManager.SeedConference(conference4);
            await _context.TestDataManager.SeedConference(conference5);
            await _context.TestDataManager.SeedConference(conference6);
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
                .WithMessages(3)
                .Build();
            conferenceType.GetProperty("ClosedDateTime")?.SetValue(conference1, DateTime.UtcNow.AddMinutes(-20));
            conferenceList.Add(conference1);

            var conference2 = new ConferenceBuilder(true, scheduledDateTime: oldHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .Build();
            conferenceType.GetProperty("ClosedDateTime")?.SetValue(conference1, DateTime.UtcNow.AddMinutes(-40));
            conferenceList.Add(conference2);

            var conference3 = new ConferenceBuilder(true, scheduledDateTime: oldHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Paused)
                .WithMessages(3)
                .Build();
            conferenceList.Add(conference3);

            var conference4 = new ConferenceBuilder(true, scheduledDateTime: oldHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .WithMessages(3)
                .Build();
            conferenceType.GetProperty("ClosedDateTime")?.SetValue(conference4, DateTime.UtcNow.AddMinutes(-20));
            conferenceList.Add(conference4);

            var conference5 = new ConferenceBuilder(true, scheduledDateTime: oldHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.InSession)
                .WithMessages(3)
                .Build();
            conferenceList.Add(conference5);

            var conference6 = new ConferenceBuilder(true, scheduledDateTime: oldHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .Build();
            conferenceList.Add(conference6);

            foreach (var c in conferenceList)
            {
                if (c.ClosedDateTime != null &&
                    c.ClosedDateTime.Value.ToUniversalTime() < DateTime.Now.ToUniversalTime().AddMinutes(-30) &&
                    c.InstantMessageHistory.Count > 0)
                {
                    _context.Test.ClosedConferencesWithMessages.Add(c);
                }

                _context.Test.Conferences.Add(await _context.TestDataManager.SeedConference(c));
            }

            var alert1 = new Alert(conference1.Id,conference1.Id, "Disconnected", TaskType.Participant);
            var alert2 = new Alert(conference2.Id,conference2.Id, "Disconnected", TaskType.Participant);
            var alert3 = new Alert(conference3.Id,conference3.Id, "Disconnected", TaskType.Participant);
            var alert4 = new Alert(conference4.Id,conference4.Id, "Disconnected", TaskType.Participant);
            var alert5 = new Alert(conference5.Id,conference5.Id, "Disconnected", TaskType.Participant);
            var alert6 = new Alert(conference6.Id,conference6.Id, "Disconnected", TaskType.Participant);

            _context.Test.Alerts = await _context.TestDataManager.SeedAlerts(new List<Alert>
            {
                alert1, alert2, alert3, alert4, alert5, alert6
            });
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
                .WithMessages(3)
                .Build();
            conferenceType.GetProperty("ClosedDateTime")?.SetValue(conference1, DateTime.UtcNow.AddMonths(-3));
            conferenceList.Add(conference1);

            var conference2 = new ConferenceBuilder(true, scheduledDateTime: oldHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .Build();
            conferenceType.GetProperty("ClosedDateTime")?.SetValue(conference1, DateTime.UtcNow.AddMonths(-2));
            conferenceList.Add(conference2);

            var conference3 = new ConferenceBuilder(true, scheduledDateTime: oldHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .WithMessages(3)
                .Build();
            conferenceType.GetProperty("ClosedDateTime")?.SetValue(conference3, DateTime.UtcNow.AddMonths(-1));
            conferenceList.Add(conference3);

            foreach (var c in conferenceList)
            {
                _context.Test.Conferences.Add(await _context.TestDataManager.SeedConference(c));
            }

            var alert1 = new Alert(conference1.Id, conference1.Id, "Disconnected", TaskType.Participant);
            var alert2 = new Alert(conference2.Id, conference2.Id, "Disconnected", TaskType.Participant);
            var alert3 = new Alert(conference3.Id, conference3.Id, "Disconnected", TaskType.Participant);
            
            _context.Test.Alerts = await _context.TestDataManager.SeedAlerts(new List<Alert>
            {
                alert1, alert2, alert3
            });
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
                .Build();
            conferenceType.GetProperty("ClosedDateTime")?.SetValue(conference1, DateTime.UtcNow.AddMinutes(-30));
            conferenceList.Add(conference1);

            var conference2 = new ConferenceBuilder(true, scheduledDateTime: oldHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .Build();
            conferenceType.GetProperty("ClosedDateTime")?.SetValue(conference1, DateTime.UtcNow.AddMinutes(-31));
            conferenceList.Add(conference2);

            var conference3 = new ConferenceBuilder(true, scheduledDateTime: oldHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .Build();
            conferenceType.GetProperty("ClosedDateTime")?.SetValue(conference3, DateTime.UtcNow.AddMinutes(-29));
            conferenceList.Add(conference3);

            foreach (var c in conferenceList)
            {
                _context.Test.Conferences.Add(await _context.TestDataManager.SeedConference(c));
            }

            var alert1 = new Alert(conference1.Id, conference1.Id, "Disconnected", TaskType.Participant);
            var alert2 = new Alert(conference2.Id, conference2.Id, "Disconnected", TaskType.Participant);
            var alert3 = new Alert(conference3.Id, conference3.Id, "Disconnected", TaskType.Participant);

            _context.Test.Alerts = await _context.TestDataManager.SeedAlerts(new List<Alert>
            {
                alert1, alert2, alert3
            });
        }

        [Given(@"I have a conference closed over (.*) months ago")]
        public async Task GivenIHaveAConferenceClosedOverMonthsAgo(int p0)
        {
            var conferenceList = new List<Domain.Conference>();
            var conferenceType = typeof(Domain.Conference);
            var utcDate = DateTime.UtcNow;
            var hearingClosed3Months = utcDate.AddMonths(-3).AddMinutes(-50);

            var conference1 = new ConferenceBuilder(true, scheduledDateTime: hearingClosed3Months)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .Build();
            conferenceType.GetProperty("ClosedDateTime").SetValue(conference1, DateTime.UtcNow.AddMonths(-3).AddMinutes(-10));
            conferenceList.Add(conference1);
            _context.Test.Conference = conference1;

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
                .WithMessages(3)
                .Build();
            conferenceList.Add(conference3);

            var conference4 = new ConferenceBuilder(true, scheduledDateTime: oldHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.InSession)
                .WithMessages(3)
                .Build();
            conferenceType.GetProperty("ClosedDateTime")?.SetValue(conference4, DateTime.UtcNow.AddMinutes(-30));
            conferenceList.Add(conference4);

            var conference5 = new ConferenceBuilder(true, scheduledDateTime: oldHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.InSession)
                .WithMessages(3)
                .Build();
            conferenceList.Add(conference5);

            var conference6 = new ConferenceBuilder(true, scheduledDateTime: oldHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Suspended)
                .Build();
            conferenceList.Add(conference6);

            foreach (var c in conferenceList)
            {
                _context.Test.Conferences.Add(await _context.TestDataManager.SeedConference(c));
            }

            var alert3 = new Alert(conference3.Id, conference3.Id, "Disconnected", TaskType.Participant);
            var alert4 = new Alert(conference4.Id, conference4.Id, "Disconnected", TaskType.Participant);
            var alert5 = new Alert(conference5.Id, conference5.Id, "Disconnected", TaskType.Participant);
            var alert6 = new Alert(conference6.Id, conference6.Id, "Disconnected", TaskType.Participant);

            _context.Test.Alerts = await _context.TestDataManager.SeedAlerts(new List<Alert>
            {
                alert3, alert4, alert5, alert6
            });
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
            NUnit.Framework.TestContext.WriteLine($"Status Code: {_context.Response.StatusCode}");
        }

        [Then(@"the response message should read '(.*)'")]
        [Then(@"the error response message should contain '(.*)'")]
        [Then(@"the error response message should also contain '(.*)'")]
        public async Task ThenTheResponseShouldContain(string errorMessage)
        {
            var messageString = await _context.Response.Content.ReadAsStringAsync();
            messageString.Should().Contain(errorMessage);
        }
    }
}
