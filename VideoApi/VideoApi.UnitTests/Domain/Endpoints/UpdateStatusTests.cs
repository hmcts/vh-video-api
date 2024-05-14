using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Domain.Endpoints
{
    public class UpdateStatusTests
    {
        [TestCase(EndpointState.Connected)]
        [TestCase(EndpointState.Disconnected)]
        [TestCase(EndpointState.InConsultation)]
        public void should_update_status(EndpointState newState)
        {
            var endpoint = new Endpoint("old name", "123@sip.com", "1234", "Defence Sol");
            endpoint.State.Should().Be(EndpointState.NotYetJoined);
            endpoint.UpdateStatus(newState);
            endpoint.State.Should().Be(newState);
        }
    }
}
