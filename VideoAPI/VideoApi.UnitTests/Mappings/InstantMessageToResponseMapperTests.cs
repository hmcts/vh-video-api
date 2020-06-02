using FluentAssertions;
using NUnit.Framework;
using Video.API.Mappings;
using VideoApi.Domain;

namespace VideoApi.UnitTests.Mappings
{
    public class InstantMessageToResponseMapperTests
    {
        [Test]
        public void Should_map_all_properties()
        {
            const string from = "Sender name";
            const string messageText = "some test message for mapping tests";
            const string to = "Receiver name";

            var instantMessage = new InstantMessage(from, messageText, to);
            var response = InstantMessageToResponseMapper.MapMessageToResponse(instantMessage);
            response.From.Should().Be(instantMessage.From);
            response.MessageText.Should().Be(instantMessage.MessageText);
            response.To.Should().Be(instantMessage.To);
        }
    }
}
