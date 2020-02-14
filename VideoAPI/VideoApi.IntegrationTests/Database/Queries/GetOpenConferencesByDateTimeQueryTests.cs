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
    public class GetOpenConferencesByDateTimeQueryTests : DatabaseTestsBase
    {
        private GetExpiredUnclosedConferencesHandler _handler;
        private Guid _conference1Id;
        private Guid _conference2Id;
        private Guid _conference3Id;
        private Guid _conference4Id;
        
        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new GetExpiredUnclosedConferencesHandler(context);
            _conference1Id = Guid.Empty;
            _conference2Id = Guid.Empty;
            _conference3Id = Guid.Empty;
            _conference4Id = Guid.Empty;
        }
        
        [TearDown]
        public async Task TearDown()
        {
            TestContext.WriteLine("Cleaning conferences for GetExpiredUnclosedConferencesHandler");
            await TestDataManager.RemoveConference(_conference1Id);
            await TestDataManager.RemoveConference(_conference2Id);
            await TestDataManager.RemoveConference(_conference3Id);
            await TestDataManager.RemoveConference(_conference4Id);
        }

        [Test]
        public async Task should_return_conferences_still_open_after_14_hours()
        {
            var utcDate = DateTime.UtcNow;
            var expired = utcDate.AddHours(-15);
            var withinTimeLimit = utcDate.AddHours(-10);
            var beyondTimeLimit = utcDate.AddHours(10);
            
            var conference1 = new ConferenceBuilder(true, scheduledDateTime: expired)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.InSession)
                .Build();
            _conference1Id = conference1.Id;
            
            var conference2 = new ConferenceBuilder(true, scheduledDateTime: withinTimeLimit)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.InSession)
                .Build();
            _conference2Id = conference2.Id;
            
            var conference3 = new ConferenceBuilder(true, scheduledDateTime: beyondTimeLimit)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Paused)
                .Build();
            _conference3Id = conference3.Id;
            
            var conference4 = new ConferenceBuilder(true, scheduledDateTime: expired)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .Build();
            _conference4Id = conference4.Id;
            
            await TestDataManager.SeedConference(conference1);
            await TestDataManager.SeedConference(conference2);
            await TestDataManager.SeedConference(conference3);
            await TestDataManager.SeedConference(conference4);

            var conferences = await _handler.Handle(new GetExpiredUnclosedConferencesQuery());
            var confIds = conferences.Select(x => x.Id).ToList();
            confIds.Should().Contain(conference1.Id);
            
            var notExpectedConferences = new List<Guid>{conference2.Id, conference3.Id, conference4.Id};
            confIds.Should()
                .NotContain(notExpectedConferences);
        }
    }
}
