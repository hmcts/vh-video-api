using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Domain.Conference
{
    public class IsClosedTests
    {
        [TestCase(ConferenceState.Closed, true)]
        [TestCase(ConferenceState.Paused, false)]
        [TestCase(ConferenceState.Suspended, false)]
        [TestCase(ConferenceState.InSession, false)]
        public void Should_check_if_closed(ConferenceState state, bool isClosed)
        {
            var conference = new ConferenceBuilder()
                .WithConferenceStatus(state).Build();
            conference.IsClosed().Should().Be(isClosed);
        }

        [Test]
        public void Should_not_be_closed_by_default()
        {
            var conference = new ConferenceBuilder().Build();
            conference.IsClosed().Should().BeFalse();
        }
    }
}
