using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;

namespace VideoApi.UnitTests.Domain.Conference
{
    public class ClearMessagesTests
    {
        [Test]
        public void should_remove_all_messages_from_hearing()
        {
            var conference = new ConferenceBuilder()
                .WithMessages(5)
                .Build();

            var beforeCount = conference.GetMessages().Count;

            //Act
            conference.ClearMessages();

            //Assert
            var afterCount = conference.GetMessages().Count;
            afterCount.Should().BeLessThan(beforeCount);
            afterCount.Should().Be(0);
        }
    }
}
