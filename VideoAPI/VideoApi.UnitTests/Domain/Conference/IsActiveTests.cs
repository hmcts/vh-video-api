using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Domain.Conference
{
    public class IsActiveTests
    {
        [TestCase(ConferenceState.Closed, false)]
        [TestCase(ConferenceState.Paused, true)]
        [TestCase(ConferenceState.Suspended, true)]
        [TestCase(ConferenceState.InSession, true)]
        public void should_check_if_active(ConferenceState state, bool isActive)
        {
            var conference = new ConferenceBuilder()
                .WithConferenceStatus(state).Build();
            conference.IsActive().Should().Be(isActive);
        }
        
        [Test]
        public void should_not_be_active_by_default()
        {
            var conference = new ConferenceBuilder().Build();
            conference.IsActive().Should().BeFalse();
        }
    }
}