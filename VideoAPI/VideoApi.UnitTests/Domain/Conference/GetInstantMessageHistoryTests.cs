using FluentAssertions;
using NUnit.Framework;
using System.Linq;
using Testing.Common.Helper.Builders.Domain;

namespace VideoApi.UnitTests.Domain.Conference
{
    public class GetInstantMessageHistoryTests
    {
        [Test]
        public void Should_get_all_messages_from_hearing()
        {
            var conference = new ConferenceBuilder().Build();
            var beforeCount = conference.GetInstantMessageHistory().Count;

            var from = "Display name";
            var messageText = "test message";
            conference.AddInstantMessage(from, messageText);

            //Act
            var messages = conference.GetInstantMessageHistory();

            //Assert
            var afterCount = messages.Count;
            afterCount.Should().BeGreaterThan(beforeCount);
            var messageSaved = messages.First();
            messageSaved.From.Should().Be(from);
            messageSaved.MessageText.Should().Be(messageText);
        }
    }
}
