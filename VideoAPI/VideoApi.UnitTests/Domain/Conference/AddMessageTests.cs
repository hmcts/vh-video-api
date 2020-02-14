using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;

namespace VideoApi.UnitTests.Domain.Conference
{
    public class AddMessageTests
    {
        [Test]
        public void should_add_new_message_to_hearing()
        {
            var conference = new ConferenceBuilder().Build();
            var beforeCount = conference.GetMessages().Count;
            var from = "Display Name";
            var message = "Test message";
            conference.AddMessage(from, message);

            var afterCount = conference.GetMessages().Count;
            afterCount.Should().BeGreaterThan(beforeCount);
        }
    }
}
