using System;
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
    public class GetConferenceByHearingRefIdQueryTests : DatabaseTestsBase
    {
        private GetConferenceByHearingRefIdQueryHandler _handler;
        private Guid _newConferenceId1;
        private Guid _newConferenceId2;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new GetConferenceByHearingRefIdQueryHandler(context);
            _newConferenceId1 = Guid.Empty;
            _newConferenceId2 = Guid.Empty;
        }

        [Test]
        public async Task should_get_conference_details_by_hearing_ref_id()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId1 = seededConference.Id;

            var conference = await _handler.Handle(new GetConferenceByHearingRefIdQuery(seededConference.HearingRefId));

            AssertConference(conference, seededConference);
        }

        [Test]
        public async Task should_get_non_closed_conference()
        {
            var knownHearingRefId = Guid.NewGuid();
            var conference1 = new ConferenceBuilder(true, knownHearingRefId)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .Build();
            _newConferenceId1 = conference1.Id;

            var conference2 = new ConferenceBuilder(true, knownHearingRefId)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.InSession)
                .Build();
            _newConferenceId2 = conference2.Id;

            await TestDataManager.SeedConference(conference1);
            await TestDataManager.SeedConference(conference2);

            var conference = await _handler.Handle(new GetConferenceByHearingRefIdQuery(knownHearingRefId));

            AssertConference(conference, conference2, true);
        }

        private void AssertConference(Conference actual, Conference expected, bool ignoreParticipants = false)
        {
            actual.Should().NotBeNull();

            actual.CaseType.Should().Be(expected.CaseType);
            actual.CaseNumber.Should().Be(expected.CaseNumber);
            actual.ScheduledDuration.Should().Be(expected.ScheduledDuration);
            actual.ScheduledDateTime.Should().Be(expected.ScheduledDateTime);
            actual.HearingRefId.Should().Be(expected.HearingRefId);

            if (ignoreParticipants) return;
            
            var participants = actual.GetParticipants();
            participants.Should().NotBeNullOrEmpty();
            foreach (var participant in participants)
            {
                participant.Name.Should().NotBeNullOrEmpty();
                participant.Username.Should().NotBeNullOrEmpty();
                participant.DisplayName.Should().NotBeNullOrEmpty();
                participant.ParticipantRefId.Should().NotBeEmpty();
                participant.UserRole.Should().NotBe(UserRole.None);
                participant.CaseTypeGroup.Should().NotBeNullOrEmpty();
            }
        }

        [TearDown]
        public async Task TearDown()
        {
            await TestDataManager.RemoveConference(_newConferenceId1);

            if (_newConferenceId2 != Guid.Empty)
            {
                await TestDataManager.RemoveConference(_newConferenceId2);
            }
        }
    }
}
