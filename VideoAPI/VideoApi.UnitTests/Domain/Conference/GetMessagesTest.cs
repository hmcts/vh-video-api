using Faker;
using FluentAssertions;
using NUnit.Framework;
using System.Linq;
using Testing.Common.Helper.Builders.Domain;

namespace VideoApi.UnitTests.Domain.Conference
{
    public class GetMessagesTest
    {
        [Test]
        public void should_get_all_messages_from_hearing()
        {
            var conference = new ConferenceBuilder().Build();
            var beforeCount = conference.GetMessages().Count;

            var from = Internet.Email();
            var to = Internet.Email();
            var messageText = Internet.DomainWord();
            conference.AddMessage(from, to, messageText);

            //Act
            var messages = conference.GetMessages();

            //Assert
            var afterCount = messages.Count;
            afterCount.Should().BeGreaterThan(beforeCount);
            var messageSaved = messages.First();
            messageSaved.From.Should().Be(from);
            messageSaved.To.Should().Be(to);
            messageSaved.MessageText.Should().Be(messageText);
        }
    }
}
