using System;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Domain.Conference
{
    public class CloseConferenceTests
    {
        [Test]
        public void Should_update_close_time_when_updating_status_to_closed()
        {
            var beforeActionTime = DateTime.UtcNow;
            var conference = new ConferenceBuilder()
                .WithParticipant(UserRole.Individual, "Applicant")
                .Build();
            
            conference.GetCurrentStatus().Should().Be(ConferenceState.NotStarted);
            conference.ClosedDateTime.Should().BeNull();
            conference.CloseConference();
            conference.ClosedDateTime.Should().NotBeNull();
            conference.ClosedDateTime.Value.Should().BeAfter(beforeActionTime);
            conference.GetCurrentStatus().Should().Be(ConferenceState.Closed);
        }

        [Test]
        public void Should_add_conference_status()
        {
            var conference = new ConferenceBuilder()
                .WithParticipant(UserRole.Individual, "Applicant")
                .Build();

            conference.GetCurrentStatus().Should().Be(ConferenceState.NotStarted);
            var beforeCount = conference.GetConferenceStatuses().Count;

            conference.CloseConference();
            var afterCount = conference.GetParticipants().Count;
            afterCount.Should().BeGreaterThan(beforeCount);

            conference.GetCurrentStatus().Should().Be(ConferenceState.Closed);
        }
    }
}
