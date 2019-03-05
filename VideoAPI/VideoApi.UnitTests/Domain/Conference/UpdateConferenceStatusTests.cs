using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Domain.Conference
{
    public class UpdateConferenceStatusTests
    {
        [Test]
        public void should_add_conference_status()
        {
            var conference = new ConferenceBuilder()
                .WithParticipant("Claimant LIP", "Claimant")
                .Build();

            conference.GetCurrentStatus().Should().BeNull();
            var beforeCount = conference.GetConferenceStatuses().Count;

            var conferenceState = ConferenceState.InSession; 
            conference.UpdateConferenceStatus(conferenceState);
            var afterCount = conference.GetParticipants().Count;
            afterCount.Should().BeGreaterThan(beforeCount);

            conference.GetCurrentStatus().ConferenceState.Should().Be(conferenceState);
        }
    }
}