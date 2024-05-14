using Testing.Common.Helper.Builders.Domain;

namespace VideoApi.UnitTests.Domain.Conference
{
    public class AddInstantMessageTests
    {
        [Test]
        public void Should_add_new_message_to_hearing()
        {
            var conference = new ConferenceBuilder().Build();
            var beforeCount = conference.GetInstantMessageHistory().Count;
            var from = "Display Name";
            var message = "Test message";
            var to = "Receiver Display Name";
            conference.AddInstantMessage(from, message, to);

            var afterCount = conference.GetInstantMessageHistory().Count;
            afterCount.Should().BeGreaterThan(beforeCount);
        }
    }
}
