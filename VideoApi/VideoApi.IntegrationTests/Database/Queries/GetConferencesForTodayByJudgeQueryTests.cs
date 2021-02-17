using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Queries
{
    public class GetConferencesForTodayByJudgeQueryTests : DatabaseTestsBase
    {
        private GetConferencesForTodayByJudgeQueryHandler _handler;
        private Guid _newConferenceId1;
        private Guid _newConferenceId2;
        private Guid _newConferenceId3;
        private Guid _newConferenceId4;
        private Guid _newConferenceId5;
        private Guid _newConferenceId6;
        private Guid _newConferenceId7;
        private Guid _newConferenceId8;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new GetConferencesForTodayByJudgeQueryHandler(context);
            _newConferenceId1 = Guid.Empty;
            _newConferenceId2 = Guid.Empty;
            _newConferenceId3 = Guid.Empty;
            _newConferenceId4 = Guid.Empty;
            _newConferenceId5 = Guid.Empty;
            _newConferenceId6 = Guid.Empty;
            _newConferenceId7 = Guid.Empty;
            _newConferenceId8 = Guid.Empty;
        }

        [Test]
        public async Task Should_get_conference_with_meeting_room_for_judge()
        {
            var username = "Automation_knownuser@hmcts.net";
            var conference1 = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Representative, "Respondent", username)
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            _newConferenceId1 = conference1.Id;

            var conference2 = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Representative, "Respondent", username)
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.InSession)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            _newConferenceId2 = conference2.Id;

            var conference3 = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Representative, "Respondent", username)
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Paused)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            _newConferenceId3 = conference3.Id;

            var conference4 = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Representative, "Respondent", username)
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Suspended)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            _newConferenceId4 = conference4.Id;

            var conference5 = new ConferenceBuilder(true, null, DateTime.UtcNow.AddDays(-1))
                .WithParticipant(UserRole.Representative, "Respondent", username)
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            _newConferenceId5 = conference5.Id;

            var conference6 = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            _newConferenceId6 = conference6.Id;

            var conference7 = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Representative, "Respondent", username)
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.InSession)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            _newConferenceId7 = conference7.Id;

            var conference8 = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Representative, "Respondent", username)
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.InSession)
                .Build();
            _newConferenceId8 = conference8.Id;

            await TestDataManager.SeedConference(conference1);
            await TestDataManager.SeedConference(conference2);
            await TestDataManager.SeedConference(conference3);
            await TestDataManager.SeedConference(conference4);
            await TestDataManager.SeedConference(conference5);
            await TestDataManager.SeedConference(conference6);
            await TestDataManager.SeedConference(conference7);
            await TestDataManager.SeedConference(conference8);

            var expectedConferences = new List<Conference> {conference1, conference2, conference3, conference4, conference7};
            var conferences = await _handler.Handle(new GetConferencesForTodayByJudgeQuery(username));

            conferences.Should().NotBeEmpty();
            conferences.Select(x => x.Id).Should().BeEquivalentTo(expectedConferences.Select(x => x.Id));
        }

        [TearDown]
        public async Task TearDown()
        {
            TestContext.WriteLine("Cleaning conferences for GetConferencesForTodayByJudgeQueryTests");
            await TestDataManager.RemoveConference(_newConferenceId1);
            await TestDataManager.RemoveConference(_newConferenceId2);
            await TestDataManager.RemoveConference(_newConferenceId3);
            await TestDataManager.RemoveConference(_newConferenceId4);
            await TestDataManager.RemoveConference(_newConferenceId5);
            await TestDataManager.RemoveConference(_newConferenceId6);
            await TestDataManager.RemoveConference(_newConferenceId7);
            await TestDataManager.RemoveConference(_newConferenceId8);
        }
    }
}
