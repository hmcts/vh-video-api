using System.Linq;
using Testing.Common.Helper.Builders.Domain;

namespace VideoApi.UnitTests.Domain.Conference
{
    public class GetInstantMessageHistoryForParticipantTests
    {
        [Test]
        public void should_get_all_messages_for_the_participant_for_the_conference()
        {
            var conference = new ConferenceBuilder().Build();

            var from = "sender display name";
            var messageText = "test message";
            var to = "receiver display name";

            var beforeCount = conference.GetInstantMessageHistoryFor(from).Count;

            conference.AddInstantMessage(from, messageText, to);

            var messages = conference.GetInstantMessageHistoryFor(from);
            var afterCount = messages.Count;
            afterCount.Should().BeGreaterThan(beforeCount);
            var messageSaved = messages.First();
            messageSaved.From.Should().Be(from);
            messageSaved.MessageText.Should().Be(messageText);
        }

        [Test]
        public void should_not_return_any_messages_for_the_a_participant_not_on_the_conference()
        {
            var nonExistentUser = "otherUser";

            var conference = new ConferenceBuilder().Build();

            var from = "sender display name";
            var messageText = "test message";
            var to = "receiver display name";

            var beforeCount = conference.GetInstantMessageHistoryFor(nonExistentUser).Count;
            conference.AddInstantMessage(from, messageText, to);
            var messages = conference.GetInstantMessageHistoryFor(nonExistentUser);
            var afterCount = messages.Count;
            afterCount.Should().Be(beforeCount);
        }
    }
}
