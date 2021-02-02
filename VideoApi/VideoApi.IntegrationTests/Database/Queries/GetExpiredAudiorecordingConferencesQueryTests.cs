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
    public class GetExpiredAudiorecordingConferencesQueryTests : DatabaseTestsBase
    {
        private GetExpiredAudiorecordingConferencesHandler _handler;
        private Guid _conference1Id;
        private Guid _conference2Id;
        private Guid _conference3Id;
        private Guid _conference4Id;
        private Guid _conference5Id;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new GetExpiredAudiorecordingConferencesHandler(context);
            _conference1Id = Guid.Empty;
            _conference2Id = Guid.Empty;
            _conference3Id = Guid.Empty;
            _conference4Id = Guid.Empty;
            _conference5Id = Guid.Empty;
        }

        [TearDown]
        public async Task TearDown()
        {
            TestContext.WriteLine("Cleaning conferences for GetExpiredAudiorecordingConferencesHandler");
            await TestDataManager.RemoveConference(_conference1Id);
            await TestDataManager.RemoveConference(_conference2Id);
            await TestDataManager.RemoveConference(_conference3Id);
            await TestDataManager.RemoveConference(_conference4Id);
            await TestDataManager.RemoveConference(_conference5Id);
        }

        [Test]
        public async Task Should_return_conferences_with_audiorecording_expired_14_hours_ago()
        {
            var utcDate = DateTime.UtcNow;
            var expired = utcDate.AddHours(-15);
            var withinTimeLimit = utcDate.AddHours(-10);
            var beyondTimeLimit = utcDate.AddHours(10);
            var beyondTimeLimitMax = utcDate.AddHours(-39);

            var conference1 = GetTestDataConference(expired, true, ConferenceState.Closed);
            _conference1Id = conference1.Id;

            var conference2 = GetTestDataConference(withinTimeLimit, false, ConferenceState.InSession);
            _conference2Id = conference2.Id;

            var conference3 = GetTestDataConference(beyondTimeLimit, false, ConferenceState.Paused);
            _conference3Id = conference3.Id;

            var conference4 = GetTestDataConference(expired, true, ConferenceState.Paused);
            _conference4Id = conference4.Id;

            var conference5 = GetTestDataConference(beyondTimeLimitMax, true, ConferenceState.Closed);
            _conference5Id = conference5.Id;

            await TestDataManager.SeedConference(conference1);
            await TestDataManager.SeedConference(conference2);
            await TestDataManager.SeedConference(conference3);
            await TestDataManager.SeedConference(conference4);
            await TestDataManager.SeedConference(conference5);

            var conferences = await _handler.Handle(new GetExpiredAudiorecordingConferencesQuery());
            var confIds = conferences.Select(x => x.Id).ToList();
            confIds.Should().Contain(conference1.Id);
            confIds.Should().Contain(conference4.Id);

            var notExpectedConferences = new List<Guid> { conference2.Id, conference3.Id, conference5.Id };
            confIds.Should()
                .NotContain(notExpectedConferences);
        }


        private Domain.Conference GetTestDataConference(DateTime startTime, bool audioRecording, ConferenceState conferenceStatus)
        {
            var conference = new ConferenceBuilder(true, scheduledDateTime: startTime)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(conferenceStatus)
                .WithAudioRecordingRequired(audioRecording)
                .Build();

            return conference;
        }
    }
}
