using System;
using System.Collections.Generic;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL;
using VideoApi.DAL.Queries;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Queries
{
    public class GetConferencesTodayQueryTests : DatabaseTestsBase
    {
        private GetConferencesTodayQueryHandler _handler;
        
        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new GetConferencesTodayQueryHandler(context);
        }
        
        [TearDown]
        public async Task TearDown()
        {
            await TestDataManager.CleanUpSeededData();
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
            
            var conference2 = new ConferenceBuilder(true, scheduledDateTime: today)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.InSession)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            
            var conference3 = new ConferenceBuilder(true, scheduledDateTime: tomorrow)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Paused)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            
            var conference4 = new ConferenceBuilder(true, scheduledDateTime: yesterday)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Suspended)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            
            var conference5 = new ConferenceBuilder(true, scheduledDateTime: tomorrow)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Suspended)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            
            var conference6 = new ConferenceBuilder(true, scheduledDateTime: today)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            
            var conference7 = new ConferenceBuilder(true, scheduledDateTime: today)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.InSession)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            
            
            var conference8 = new ConferenceBuilder(true, scheduledDateTime: today)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.InSession)
                .Build();
            
            await TestDataManager.SeedConference(conference1);
            await TestDataManager.SeedConference(conference2);
            await TestDataManager.SeedConference(conference3);
            await TestDataManager.SeedConference(conference4);
            await TestDataManager.SeedConference(conference5);
            await TestDataManager.SeedConference(conference6);
            await TestDataManager.SeedConference(conference7);
            await TestDataManager.SeedConference(conference8);

            var conferences = await _handler.Handle(new GetConferencesTodayQuery());

            conferences.Should().NotBeEmpty();
            foreach (var conference in conferences)
            {
                conference.ScheduledDateTime.DayOfYear.Should().Be(DateTime.UtcNow.DayOfYear);
                conference.MeetingRoom.IsSet().Should().BeTrue();
            }
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
            
            var conference2 = new ConferenceBuilder(true, venueName: hearingVenueName1)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            
            var conference3 = new ConferenceBuilder(true, venueName: hearingVenueName2)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            
            var conference4 = new ConferenceBuilder(true, venueName: hearingVenueName2)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            
            var conference5 = new ConferenceBuilder(true, venueName: hearingVenueName3)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            
            var conference6 = new ConferenceBuilder(true, venueName: hearingVenueName3)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .Build();
            
            await TestDataManager.SeedConference(conference1);
            await TestDataManager.SeedConference(conference2);
            await TestDataManager.SeedConference(conference3);
            await TestDataManager.SeedConference(conference4);
            await TestDataManager.SeedConference(conference5);
            await TestDataManager.SeedConference(conference6);

            var result = await _handler.Handle(new GetConferencesTodayQuery
            {
                HearingVenueNames = new List<string> { hearingVenueName1, hearingVenueName2 }
            });
            result.Should().NotBeEmpty();
            result.Count.Should().Be(4);
            result.Should().BeInAscendingOrder(c => c.ScheduledDateTime);
        }
    }
}
