using System;
using System.Collections.Generic;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Queries
{
    public class GetNonClosedConferenceByHearingRefIdQueryTests : DatabaseTestsBase
    {
        private GetNonClosedConferenceByHearingRefIdQueryHandler _handler;
        private Guid _newConferenceId1;
        private Guid _newConferenceId2;
        
        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new GetNonClosedConferenceByHearingRefIdQueryHandler(context);
            _newConferenceId1 = Guid.Empty;
            _newConferenceId2 = Guid.Empty;
        }
        
        [Test]
        public async Task Should_get_conference_details_by_hearing_ref_id()
        {
            var knownHearingRefId = Guid.NewGuid();
            var seededConference = new ConferenceBuilder(true, knownHearingRefId)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.InSession)
                .WithEndpoints(new List<Endpoint>
                    {
                        new ("one", "44564", "1234"),
                        new ("two", "867744", "5678")
                    })
                .Build();
            _newConferenceId1 = seededConference.Id;
            await TestDataManager.SeedConference(seededConference);
            var conference = await _handler.Handle(new GetNonClosedConferenceByHearingRefIdQuery(knownHearingRefId));

            AssertConference(conference[0], conference[0]);
        }
        
        [Test]
        public async Task Should_get_non_closed_conference()
        {
            var knownHearingRefId = Guid.NewGuid();
            var conference1 = new ConferenceBuilder(true, knownHearingRefId)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .Build();
            _newConferenceId1 = conference1.Id;

            var conference2 = new ConferenceBuilder(true, knownHearingRefId)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.InSession)
                .Build();
            _newConferenceId2 = conference2.Id;

            await TestDataManager.SeedConference(conference1);
            await TestDataManager.SeedConference(conference2);

            var conference = await _handler.Handle(new GetNonClosedConferenceByHearingRefIdQuery(knownHearingRefId));

            AssertConference(conference[0], conference2, true);
        }
        
        [Test]
        public async Task Should_get_closed_conference_when_toggle_set_to_true()
        {
            var knownHearingRefId = Guid.NewGuid();
            var conference1 = new ConferenceBuilder(true, knownHearingRefId)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .Build();
            _newConferenceId1 = conference1.Id;

            await TestDataManager.SeedConference(conference1);

            var conference = await _handler.Handle(new GetNonClosedConferenceByHearingRefIdQuery(knownHearingRefId,true));

            AssertConference(conference[0], conference1, true);
        }
        
        private static void AssertConference(Conference actual, Conference expected, bool ignoreParticipants = false)
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
                participant.Username.Should().NotBeNullOrEmpty();
                participant.DisplayName.Should().NotBeNullOrEmpty();
                participant.ParticipantRefId.Should().NotBeEmpty();
                participant.UserRole.Should().NotBe(UserRole.None);
            }

            var endpoints = actual.GetEndpoints();
            endpoints.Should().NotBeNullOrEmpty();
            foreach (var endpoint in endpoints)
            {
                endpoint.DisplayName.Should().NotBeNullOrEmpty();
                endpoint.Pin.Should().NotBeNullOrEmpty();
                endpoint.SipAddress.Should().NotBeNullOrEmpty();
                endpoint.DefenceAdvocate.Should().NotBeEmpty();
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
