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
    public class GetConferencesByUsernameQueryTests : DatabaseTestsBase
    {
        private GetConferencesByUsernameQueryHandler _handler;
        private Guid _newConferenceId1;
        private Guid _newConferenceId2;
        private Guid _newConferenceId3;
        private Guid _newConferenceId4;
        private Guid _newConferenceId5;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new GetConferencesByUsernameQueryHandler(context);
            _newConferenceId1 = Guid.Empty;
            _newConferenceId2 = Guid.Empty;
            _newConferenceId3 = Guid.Empty;
            _newConferenceId4 = Guid.Empty;
            _newConferenceId5 = Guid.Empty;
        }

        [Test]
        public async Task should_get_conference_details_by_id()
        {
            var username = "knownuser@email.com";
            var conference1 = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Representative, "Defendant", username)
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .Build();
            _newConferenceId1 = conference1.Id;

            var conference2 = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Representative, "Defendant", username)
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.InSession)
                .Build();
            _newConferenceId2 = conference2.Id;

            var conference3 = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Representative, "Defendant", username)
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Paused)
                .Build();
            _newConferenceId3 = conference3.Id;

            var conference4 = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Representative, "Defendant", username)
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Suspended)
                .Build();
            _newConferenceId4 = conference4.Id;
            
            var conference5 = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Suspended)
                .Build();
            _newConferenceId5 = conference5.Id;

            await TestDataManager.SeedConference(conference1);
            await TestDataManager.SeedConference(conference2);
            await TestDataManager.SeedConference(conference3);
            await TestDataManager.SeedConference(conference4);
            await TestDataManager.SeedConference(conference5);

            var conferences = await _handler.Handle(new GetConferencesByUsernameQuery(username));

            conferences.Should().NotBeEmpty();
            conferences.Count.Should().Be(3);
        }

        [TearDown]
        public async Task TearDown()
        {
            TestContext.WriteLine("Cleaning conferences for GetConferencesByUsernameQueryTests");
            await TestDataManager.RemoveConference(_newConferenceId1);
            await TestDataManager.RemoveConference(_newConferenceId2);
            await TestDataManager.RemoveConference(_newConferenceId3);
            await TestDataManager.RemoveConference(_newConferenceId4);
            await TestDataManager.RemoveConference(_newConferenceId5);
        }
    }
}