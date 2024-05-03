using NUnit.Framework.Internal;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Domain.Conference
{
    public class IsConferenceAccessibleTests
    {
        [TestCase(ConferenceState.Paused)]
        [TestCase(ConferenceState.Suspended)]
        [TestCase(ConferenceState.InSession)]
        [TestCase(ConferenceState.NotStarted)]
        public void conference_should_be_accessible_when_not_closed(ConferenceState status)
        {
            var conference = new ConferenceBuilder().WithConferenceStatus(status).Build();
            conference.IsConferenceAccessible().Should().BeTrue();
        }
        
        [Test]
        public void conference_should_be_accessible_when_closed_but_for_less_than_120_minutes()
        {
            var conference = new ConferenceBuilder().WithConferenceStatus(ConferenceState.Closed).Build();
            conference.IsConferenceAccessible().Should().BeTrue();
        }
        
        [Test]
        public void conference_should_not_be_accessible_when_closed_for_more_than_120_minutes()
        {
            var conference = new ConferenceBuilder().WithConferenceStatus(ConferenceState.Closed).Build();
            conference.ClosedDateTime = conference.ClosedDateTime!.Value.AddMinutes(-121);
            conference.IsConferenceAccessible().Should().BeFalse();
        }
    }
}
