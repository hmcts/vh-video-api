using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL;
using VideoApi.DAL.Queries;
using VideoApi.Domain.Enums;

namespace VideoApi.IntegrationTests.Database.Queries
{
    public class GetJudgesInHearingsTodayQueryTests : DatabaseTestsBase
    {
        private GetJudgesInHearingsTodayQueryHandler _handler;
        private Guid _newConferenceId1;
        private Guid _newConferenceId2;
        private Guid _newConferenceId3;
        private Guid _newConferenceId4;
        private Guid _newConferenceId5;
        private Guid _newConferenceId6;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new GetJudgesInHearingsTodayQueryHandler(context);
            _newConferenceId1 = Guid.Empty;
            _newConferenceId2 = Guid.Empty;
            _newConferenceId3 = Guid.Empty;
            _newConferenceId4 = Guid.Empty;
            _newConferenceId5 = Guid.Empty;
            _newConferenceId6 = Guid.Empty;
        }

        [TearDown]
        public async Task TearDown()
        {
            TestContext.WriteLine("Cleaning conferences for GetJudgesInHearingsTodayQueryHandler");
            await TestDataManager.RemoveConference(_newConferenceId1);
            await TestDataManager.RemoveConference(_newConferenceId2);
            await TestDataManager.RemoveConference(_newConferenceId3);
            await TestDataManager.RemoveConference(_newConferenceId4);
            await TestDataManager.RemoveConference(_newConferenceId5);
            await TestDataManager.RemoveConference(_newConferenceId6);
        }

        [Test]
        public async Task Should_return_no_conferences_as_no_judges_in_hearing_or_available()
        {
            var today = DateTime.Today.AddHours(10);
            var tomorrow = DateTime.Today.AddDays(1).AddHours(10);
            var yesterday = DateTime.Today.AddDays(-1).AddHours(10);
            var conference1 = new ConferenceBuilder(true, scheduledDateTime: yesterday)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null, participantState: ParticipantState.Disconnected)
                .WithConferenceStatus(ConferenceState.Closed)
                .Build();
            _newConferenceId1 = conference1.Id;

            var conference2 = new ConferenceBuilder(true, scheduledDateTime: today)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null, participantState: ParticipantState.InConsultation)
                .WithConferenceStatus(ConferenceState.InSession)
                .Build();
            _newConferenceId2 = conference2.Id;

            var conference3 = new ConferenceBuilder(true, scheduledDateTime: tomorrow)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null, participantState: ParticipantState.NotSignedIn)
                .WithConferenceStatus(ConferenceState.Paused)
                .Build();
            _newConferenceId3 = conference3.Id;

            var conference4 = new ConferenceBuilder(true, scheduledDateTime: yesterday)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null, participantState: ParticipantState.Disconnected)
                .WithConferenceStatus(ConferenceState.Suspended)
                .Build();
            _newConferenceId4 = conference4.Id;

            var conference5 = new ConferenceBuilder(true, scheduledDateTime: tomorrow)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null, participantState: ParticipantState.Disconnected)
                .WithConferenceStatus(ConferenceState.Suspended)
                .Build();
            _newConferenceId5 = conference5.Id;

            var conference6 = new ConferenceBuilder(true, scheduledDateTime: today)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null, participantState: ParticipantState.Disconnected)
                .Build();
            _newConferenceId6 = conference6.Id;

            await TestDataManager.SeedConference(conference1);
            await TestDataManager.SeedConference(conference2);
            await TestDataManager.SeedConference(conference3);
            await TestDataManager.SeedConference(conference4);
            await TestDataManager.SeedConference(conference5);
            await TestDataManager.SeedConference(conference6);

            var conferences = await _handler.Handle(new GetJudgesInHearingsTodayQuery());

            conferences.Should().BeEmpty();
        }
        
        [Test]
        public async Task Should_return_conferences_with_judges_in_hearing_or_available()
        {
            var today = DateTime.Today.AddHours(10);
            var tomorrow = DateTime.Today.AddDays(1).AddHours(10);
            var yesterday = DateTime.Today.AddDays(-1).AddHours(10);
            var conference1 = new ConferenceBuilder(true, scheduledDateTime: yesterday)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .Build();
            _newConferenceId1 = conference1.Id;

            var conference2 = new ConferenceBuilder(true, scheduledDateTime: today)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null, participantState: ParticipantState.InHearing)
                .WithConferenceStatus(ConferenceState.InSession)
                .Build();
            _newConferenceId2 = conference2.Id;

            var conference3 = new ConferenceBuilder(true, scheduledDateTime: tomorrow)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Paused)
                .Build();
            _newConferenceId3 = conference3.Id;

            var conference4 = new ConferenceBuilder(true, scheduledDateTime: yesterday)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Suspended)
                .Build();
            _newConferenceId4 = conference4.Id;

            var conference5 = new ConferenceBuilder(true, scheduledDateTime: tomorrow)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Suspended)
                .Build();
            _newConferenceId5 = conference5.Id;

            var conference6 = new ConferenceBuilder(true, scheduledDateTime: today)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null, participantState: ParticipantState.Available)
                .Build();
            _newConferenceId6 = conference6.Id;

            await TestDataManager.SeedConference(conference1);
            await TestDataManager.SeedConference(conference2);
            await TestDataManager.SeedConference(conference3);
            await TestDataManager.SeedConference(conference4);
            await TestDataManager.SeedConference(conference5);
            await TestDataManager.SeedConference(conference6);

            var conferences = await _handler.Handle(new GetJudgesInHearingsTodayQuery());

            conferences.Should().NotBeEmpty();
            conferences.Count.Should().Be(2);
        }
    }
}
