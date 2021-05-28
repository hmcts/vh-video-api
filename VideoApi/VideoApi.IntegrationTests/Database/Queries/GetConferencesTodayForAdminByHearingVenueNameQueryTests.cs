using System;
using System.Collections.Generic;
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
    public class GetConferencesTodayForAdminByHearingVenueNameQueryTests : DatabaseTestsBase
    {
        private GetConferencesTodayForAdminByHearingVenueNameQueryHandler _handler;
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
            _handler = new GetConferencesTodayForAdminByHearingVenueNameQueryHandler(context);
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
        public async Task Should_get_conference_with_meeting_room_for_today()
        {
            var today = DateTime.Today.AddHours(10);
            var tomorrow = DateTime.Today.AddDays(1).AddHours(10);
            var yesterday = DateTime.Today.AddDays(-1).AddHours(10);
            var conference1 = new ConferenceBuilder(true, scheduledDateTime: yesterday)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            _newConferenceId1 = conference1.Id;

            var conference2 = new ConferenceBuilder(true, scheduledDateTime: today)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.InSession)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            _newConferenceId2 = conference2.Id;

            var conference3 = new ConferenceBuilder(true, scheduledDateTime: tomorrow)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Paused)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            _newConferenceId3 = conference3.Id;

            var conference4 = new ConferenceBuilder(true, scheduledDateTime: yesterday)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Suspended)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            _newConferenceId4 = conference4.Id;

            var conference5 = new ConferenceBuilder(true, scheduledDateTime: tomorrow)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Suspended)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            _newConferenceId5 = conference5.Id;

            var conference6 = new ConferenceBuilder(true, scheduledDateTime: today)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            _newConferenceId6 = conference6.Id;

            var conference7 = new ConferenceBuilder(true, scheduledDateTime: today)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.InSession)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            _newConferenceId7 = conference7.Id;


            var conference8 = new ConferenceBuilder(true, scheduledDateTime: today)
                .WithParticipant(UserRole.Representative, "Respondent")
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

            var conferences = await _handler.Handle(new GetConferencesTodayForAdminByHearingVenueNameQuery());

            conferences.Should().NotBeEmpty();
            foreach (var conference in conferences)
            {
                conference.ScheduledDateTime.DayOfYear.Should().Be(DateTime.UtcNow.DayOfYear);
                conference.MeetingRoom.IsSet().Should().BeTrue();
            }

            TestContext.WriteLine("Cleaning conferences for GetConferencesTodayForAdminQueryHandler");
            await TestDataManager.RemoveConference(_newConferenceId1);
            await TestDataManager.RemoveConference(_newConferenceId2);
            await TestDataManager.RemoveConference(_newConferenceId3);
            await TestDataManager.RemoveConference(_newConferenceId4);
            await TestDataManager.RemoveConference(_newConferenceId5);
            await TestDataManager.RemoveConference(_newConferenceId6);
            await TestDataManager.RemoveConference(_newConferenceId7);
            await TestDataManager.RemoveConference(_newConferenceId8);
        }

        [Test]
        public async Task should_get_conferences_for_today_filtered_by_judge_firstname()
        {
            var hearingVenueName1 = @"Manchester";
            var hearingVenueName2 = @"Birmingham";
            var hearingVenueName3 = @"Luton";

            var conference1 = new ConferenceBuilder(true, venueName: hearingVenueName1)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            _newConferenceId1 = conference1.Id;

            var conference2 = new ConferenceBuilder(true, venueName: hearingVenueName1)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            _newConferenceId2 = conference2.Id;

            var conference3 = new ConferenceBuilder(true, venueName: hearingVenueName2)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            _newConferenceId3 = conference3.Id;

            var conference4 = new ConferenceBuilder(true, venueName: hearingVenueName2)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            _newConferenceId4 = conference4.Id;

            var conference5 = new ConferenceBuilder(true, venueName: hearingVenueName3)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            _newConferenceId5 = conference5.Id;

            var conference6 = new ConferenceBuilder(true, venueName: hearingVenueName3)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            _newConferenceId6 = conference6.Id;

            await TestDataManager.SeedConference(conference1);
            await TestDataManager.SeedConference(conference2);
            await TestDataManager.SeedConference(conference3);
            await TestDataManager.SeedConference(conference4);
            await TestDataManager.SeedConference(conference5);
            await TestDataManager.SeedConference(conference6);

            var result = await _handler.Handle(new GetConferencesTodayForAdminByHearingVenueNameQuery
            {
                HearingVenueNames = new List<string> { hearingVenueName1, hearingVenueName2 }
            });
            result.Should().NotBeEmpty();
            result.Count.Should().Be(4);
            result.Should().BeInAscendingOrder(c => c.ScheduledDateTime);

            TestContext.WriteLine("Cleaning conferences for GetConferencesTodayForAdminQueryHandler");
            await TestDataManager.RemoveConference(_newConferenceId1);
            await TestDataManager.RemoveConference(_newConferenceId2);
            await TestDataManager.RemoveConference(_newConferenceId3);
            await TestDataManager.RemoveConference(_newConferenceId4);
            await TestDataManager.RemoveConference(_newConferenceId5);
            await TestDataManager.RemoveConference(_newConferenceId6);
        }
    }
}
