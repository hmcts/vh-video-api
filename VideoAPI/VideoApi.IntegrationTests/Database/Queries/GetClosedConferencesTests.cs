using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL;
using VideoApi.DAL.Queries;
using VideoApi.Domain.Enums;

namespace VideoApi.IntegrationTests.Database.Queries
{
    public class GetClosedConferencesTests : DatabaseTestsBase
    {
        private GetClosedConferencesWithInstantMessagesHandler _handler;
        private Guid _conference1Id;
        private Guid _conference2Id;
        private Guid _conference3Id;
        private Guid _conference4Id;
        private Guid _conference5Id;
        private Guid _conference6Id;
        private Guid _conference7Id;
        private List<Domain.Conference> conferenceList;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new GetClosedConferencesWithInstantMessagesHandler(context);
            _conference1Id = Guid.Empty;
            _conference2Id = Guid.Empty;
            _conference3Id = Guid.Empty;
            _conference4Id = Guid.Empty;
            _conference5Id = Guid.Empty;
            _conference6Id = Guid.Empty;
            _conference7Id = Guid.Empty;
        }

        [TearDown]
        public async Task TearDown()
        {
            TestContext.WriteLine("Cleaning conferences for GetClosedConferencesHandler");
            foreach (var c in conferenceList)
            {
                await TestDataManager.RemoveConference(c.Id);
            }
        }

        [Test]
        public async Task should_return_closed_conferences_with_im_messages_closed_over_30_minutes_ago()
        {
            conferenceList = new List<Domain.Conference>();
            var conferenceType = typeof(Domain.Conference);
            var utcDate = DateTime.UtcNow;
            var currentHearing = utcDate.AddMinutes(-40);
            var oldHearing = utcDate.AddMinutes(-180);
            var veryOldHearing = utcDate.AddMonths(-4);

            // conference in session.
            var conference1 = new ConferenceBuilder(true, scheduledDateTime: currentHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.InSession)
                .Build();
            _conference1Id = conference1.Id;
            conferenceList.Add(conference1);

            // closed conferences with no messages.
            var conference2 = new ConferenceBuilder(true, scheduledDateTime: oldHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .Build();
            conferenceType.GetProperty("ClosedDateTime").SetValue(conference2, DateTime.UtcNow.AddMinutes(-35));
            _conference2Id = conference2.Id;
            conferenceList.Add(conference2);

            var conference3 = new ConferenceBuilder(true, scheduledDateTime: oldHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .Build();
            conferenceType.GetProperty("ClosedDateTime").SetValue(conference3, DateTime.UtcNow.AddMinutes(-25));
            _conference3Id = conference3.Id;
            conferenceList.Add(conference3);

            // closed conferences with messages.
            var conference4 = new ConferenceBuilder(true, scheduledDateTime: oldHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithMessages(10)
                .WithConferenceStatus(ConferenceState.Closed)
                .Build();
            conferenceType.GetProperty("ClosedDateTime").SetValue(conference4, DateTime.UtcNow.AddMinutes(-31));
            _conference4Id = conference4.Id;
            conferenceList.Add(conference4);

            var conference5 = new ConferenceBuilder(true, scheduledDateTime: oldHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithMessages(10)
                .WithConferenceStatus(ConferenceState.Closed)
                .Build();
            conferenceType.GetProperty("ClosedDateTime").SetValue(conference5, DateTime.UtcNow.AddMinutes(-30));
            _conference5Id = conference5.Id;
            conferenceList.Add(conference5);

            var conference6 = new ConferenceBuilder(true, scheduledDateTime: oldHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithMessages(10)
                .WithConferenceStatus(ConferenceState.Closed)
                .Build();
            conferenceType.GetProperty("ClosedDateTime").SetValue(conference6, DateTime.UtcNow.AddMinutes(-29));
            _conference6Id = conference6.Id;
            conferenceList.Add(conference6);

            var conference7 = new ConferenceBuilder(true, scheduledDateTime: veryOldHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithMessages(10)
                .WithConferenceStatus(ConferenceState.Closed)
                .Build();
            conferenceType.GetProperty("ClosedDateTime").SetValue(conference7, DateTime.UtcNow.AddMonths(-3));
            _conference7Id = conference7.Id;
            conferenceList.Add(conference7);

            foreach (var c in conferenceList)
            {
                await TestDataManager.SeedConference(c);
            }

            var conferences = await _handler.Handle(new GetClosedConferencesWithInstantMessagesQuery());
            var confIds = conferences.Select(x => x.Id).ToList();

            var expectedConferences = new List<Guid> { conference4.Id, conference5.Id, conference7.Id };
            confIds.Should()
                .Contain(expectedConferences);

            var notExpectedConferences = new List<Guid> { conference1.Id, conference2.Id, conference3.Id, conference6.Id };
            confIds.Should()
                .NotContain(notExpectedConferences);
        }
    }
}
