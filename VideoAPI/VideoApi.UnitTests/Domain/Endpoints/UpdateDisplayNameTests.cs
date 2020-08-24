using FluentAssertions;
using NUnit.Framework;
using VideoApi.Domain;

namespace VideoApi.UnitTests.Domain.Endpoints
{
    public class UpdateDisplayNameTests
    {
        [Test]
        public void should_update_display_name()
        {
            var endpoint = new Endpoint("old name", "123@sip.com", "1234");
            const string newDisplayName = "New Auto Name";
            endpoint.UpdateDisplayName(newDisplayName);

            endpoint.DisplayName.Should().Be(newDisplayName);
        }
    }
}
