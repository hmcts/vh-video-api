using Testing.Common.Helper.Builders.Domain;

namespace VideoApi.UnitTests.Domain.Conference
{
    public class ClearInstantMessageHistoryTests
    {
        [Test]
        public void Should_remove_all_messages_from_hearing()
        {
            var conference = new ConferenceBuilder()
                .WithMessages(5)
                .Build();

            var beforeCount = conference.GetInstantMessageHistory().Count;

            //Act
            conference.ClearInstantMessageHistory();

            //Assert
            var afterCount = conference.GetInstantMessageHistory().Count;
            afterCount.Should().BeLessThan(beforeCount);
            afterCount.Should().Be(0);
        }
    }
}
