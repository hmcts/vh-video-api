using VideoApi.Contract.Enums;
using VideoApi.Domain;
using VideoApi.Mappings;

namespace VideoApi.UnitTests.Mappings
{
    public class EndpointToResponseMapperTests
    {
        [Test]
        public void should_map_endpoint_to_response()
        {
            var endpoint = new Endpoint("Display", "sip123", "1245", "Defence Sol");
            var response = EndpointToResponseMapper.MapEndpointResponse(endpoint);
            
            response.Id.Should().Be(endpoint.Id);
            response.Pin.Should().Be(endpoint.Pin);
            response.Status.Should().Be((EndpointState)endpoint.State);
            response.SipAddress.Should().Be(endpoint.SipAddress);
            response.DefenceAdvocate.Should().Be(endpoint.DefenceAdvocate);
        }
    }
}
