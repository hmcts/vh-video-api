using System;
using System.Collections.Generic;
using System.Linq;
using Faker;
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
    public class GetConferencesTodayForAdminQueryTests : DatabaseTestsBase
    {
        private GetConferencesTodayForAdminQueryHandler _handler;
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
            _handler = new GetConferencesTodayForAdminQueryHandler(context);
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
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .WithMeetingRoom("https://poc.node.com", "user@email.com")
                .Build();
            _newConferenceId1 = conference1.Id;

            var conference2 = new ConferenceBuilder(true, scheduledDateTime: today)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.InSession)
                .WithMeetingRoom("https://poc.node.com", "user@email.com")
                .Build();
            _newConferenceId2 = conference2.Id;

            var conference3 = new ConferenceBuilder(true, scheduledDateTime: tomorrow)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Paused)
                .WithMeetingRoom("https://poc.node.com", "user@email.com")
                .Build();
            _newConferenceId3 = conference3.Id;

            var conference4 = new ConferenceBuilder(true, scheduledDateTime: yesterday)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Suspended)
                .WithMeetingRoom("https://poc.node.com", "user@email.com")
                .Build();
            _newConferenceId4 = conference4.Id;

            var conference5 = new ConferenceBuilder(true, scheduledDateTime: tomorrow)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Suspended)
                .WithMeetingRoom("https://poc.node.com", "user@email.com")
                .Build();
            _newConferenceId5 = conference5.Id;

            var conference6 = new ConferenceBuilder(true, scheduledDateTime: today)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithMeetingRoom("https://poc.node.com", "user@email.com")
                .Build();
            _newConferenceId6 = conference6.Id;

            var conference7 = new ConferenceBuilder(true, scheduledDateTime: today)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.InSession)
                .WithMeetingRoom("https://poc.node.com", "user@email.com")
                .Build();
            _newConferenceId7 = conference7.Id;


            var conference8 = new ConferenceBuilder(true, scheduledDateTime: today)
                .WithParticipant(UserRole.Representative, "Defendant")
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

            var conferences = await _handler.Handle(new GetConferencesTodayForAdminQuery());

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
            var venue1 = @"Manchester";
            var venue2 = @"Birmingham";
            var venue3 = @"Luton";

            var participants1 = new List<Participant>
            {
                new Participant(Guid.NewGuid(), "", "firstJudge", "James", "Judge James", "judge.james@email.com",
                    UserRole.Judge, ParticipantBuilder.DetermineHearingRole(UserRole.Individual, "Children Act"),
                    "Children Act", Internet.Email(), Phone.Number()),
                new Participant(Guid.NewGuid(), "", "firstname", "lastname", "firstname lastname",
                    "firstname.lastname@email.com", UserRole.Individual,
                    ParticipantBuilder.DetermineHearingRole(UserRole.Individual, "Children Act"), "Children Act",
                    Internet.Email(), Phone.Number())
            };
            var participants2 = new List<Participant>
            {
                new Participant(Guid.NewGuid(), "", "secondJudge", "James II", "SecondJudge James II",
                    "secondJudge.james@email.com", UserRole.Judge,
                    ParticipantBuilder.DetermineHearingRole(UserRole.Judge, "Children Act"), "Children Act",
                    Internet.Email(), Phone.Number()),
                new Participant(Guid.NewGuid(), "", "individualFirst", "lastname", "individualFirst lastname",
                    "individualFirst.lastname@email.com", UserRole.Individual,
                    ParticipantBuilder.DetermineHearingRole(UserRole.Individual, "Children Act"), "Children Act",
                    Internet.Email(),
                    Phone.Number()),
            };
            var participants3 = new List<Participant>
            {
                new Participant(Guid.NewGuid(), "", "firstJudge", "James", "firstJudge James",
                    "firstJudge.james@email.com", UserRole.Judge,
                    ParticipantBuilder.DetermineHearingRole(UserRole.Judge, "Children Act"), "Children Act",
                    Internet.Email(), Phone.Number()),
                new Participant(Guid.NewGuid(), "", "representativeFirst", "lastname", "representativeFirst lastname",
                    "representativeFirst.lastname@email.com", UserRole.Representative,
                    ParticipantBuilder.DetermineHearingRole(UserRole.Representative, "Children Act"), "Children Act",
                    Internet.Email(),
                    Phone.Number()),
            };
            var participants4 = new List<Participant>
            {
                new Participant(Guid.NewGuid(), "", "thirdJudge", "James", "thirdJudge James",
                    "thirdJudge.james@email.com", UserRole.Judge, ParticipantBuilder.DetermineHearingRole(UserRole.Judge, "Children Act"),"Children Act", Internet.Email(), Phone.Number()),
                new Participant(Guid.NewGuid(), "", "representativeFirst", "lastname", "representativeFirst lastname",
                    "representativeFirst.lastname@email.com", UserRole.Representative, ParticipantBuilder.DetermineHearingRole(UserRole.Representative, "Children Act"),"Children Act", Internet.Email(),
                    Phone.Number()),
            };
            var participants5 = new List<Participant>
            {
                new Participant(Guid.NewGuid(), "", "thirdJudge", "James", "thirdJudge James",
                    "thirdJudge.james@email.com", UserRole.Judge, ParticipantBuilder.DetermineHearingRole(UserRole.Judge, "Children Act"),"Children Act", Internet.Email(), Phone.Number()),
                new Participant(Guid.NewGuid(), "", "representativeSecond", "lastname", "representativeSecond lastname",
                    "representativeSecond.lastname@email.com", UserRole.Representative,ParticipantBuilder.DetermineHearingRole(UserRole.Representative, "Children Act"), "Children Act",
                    Internet.Email(), Phone.Number()),
            };
            var participants6 = new List<Participant>
            {
                new Participant(Guid.NewGuid(), "", "secondJudge", "James II", "SecondJudge James II",
                    "secondJudge.james@email.com", UserRole.Judge,
                    ParticipantBuilder.DetermineHearingRole(UserRole.Judge, "Children Act"), "Children Act",
                    Internet.Email(), Phone.Number()),
                new Participant(Guid.NewGuid(), "", "representativeThird", "lastname", "representativeThird lastname",
                    "representativeThird.lastname@email.com", UserRole.Representative,
                    ParticipantBuilder.DetermineHearingRole(UserRole.Representative, "Children Act"), "Children Act",
                    Internet.Email(),
                    Phone.Number()),
            };

            var conference1 = new ConferenceBuilder(true, venueName: venue1)
                .WithParticipants(participants1)
                .WithMeetingRoom("https://poc.node.com", "user@email.com")
                .Build();
            _newConferenceId1 = conference1.Id;

            var conference2 = new ConferenceBuilder(true, venueName: venue1)
                .WithParticipants(participants2)
                .WithMeetingRoom("https://poc.node.com", "user@email.com")
                .Build();
            _newConferenceId2 = conference2.Id;

            var conference3 = new ConferenceBuilder(true, venueName: venue2)
                .WithParticipants(participants3)
                .WithMeetingRoom("https://poc.node.com", "user@email.com")
                .Build();
            _newConferenceId3 = conference3.Id;

            var conference4 = new ConferenceBuilder(true, venueName: venue2)
                .WithParticipants(participants4)
                .WithMeetingRoom("https://poc.node.com", "user@email.com")
                .Build();
            _newConferenceId4 = conference4.Id;

            var conference5 = new ConferenceBuilder(true, venueName: venue3)
                .WithParticipants(participants5)
                .WithMeetingRoom("https://poc.node.com", "user@email.com")
                .Build();
            _newConferenceId5 = conference5.Id;

            var conference6 = new ConferenceBuilder(true, venueName: venue3)
                .WithParticipants(participants6)
                .WithMeetingRoom("https://poc.node.com", "user@email.com")
                .Build();
            _newConferenceId6 = conference6.Id;

            await TestDataManager.SeedConference(conference1);
            await TestDataManager.SeedConference(conference2);
            await TestDataManager.SeedConference(conference3);
            await TestDataManager.SeedConference(conference4);
            await TestDataManager.SeedConference(conference5);
            await TestDataManager.SeedConference(conference6);

            var result = await _handler.Handle(new GetConferencesTodayForAdminQuery
            {
                UserNames = new List<string> {participants1[0].FirstName, participants4[0].FirstName}
            });
            result.Should().NotBeEmpty();
            result.Count.Should().Be(4);
            result.Should().BeInAscendingOrder(c => c.ScheduledDateTime);

            result[0].Participants.FirstOrDefault(x => x.UserRole == UserRole.Judge)?.FirstName.Should()
                .Be(participants1[0].FirstName);
            result[1].Participants.FirstOrDefault(x => x.UserRole == UserRole.Judge)?.FirstName.Should()
                .Be(participants1[0].FirstName);
            result[2].Participants.FirstOrDefault(x => x.UserRole == UserRole.Judge)?.FirstName.Should()
                .Be(participants4[0].FirstName);
            result[3].Participants.FirstOrDefault(x => x.UserRole == UserRole.Judge)?.FirstName.Should()
                .Be(participants4[0].FirstName);

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
