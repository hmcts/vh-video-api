using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;

namespace VideoApi.UnitTests.Domain.Conference
{
    public class AddInstantMessageTests
    {
        [Test]
        public void should_add_new_message_to_hearing()
        {
            var conference = new ConferenceBuilder().Build();
            var beforeCount = conference.GetInstantMessageHistory().Count;
            var from = "Display Name";
            var message = "Test message";
            conference.AddInstantMessage(from, message);

            var afterCount = conference.GetInstantMessageHistory().Count;
            afterCount.Should().BeGreaterThan(beforeCount);
        }
    }
}
