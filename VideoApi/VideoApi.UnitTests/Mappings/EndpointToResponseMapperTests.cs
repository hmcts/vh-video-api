using VideoApi.Contract.Enums;
using VideoApi.Domain;
using VideoApi.Mappings;
using LinkedParticipantType = VideoApi.Domain.Enums.LinkedParticipantType;

namespace VideoApi.UnitTests.Mappings
{
    public class EndpointToResponseMapperTests
    {
        [Test]
        public void should_map_endpoint_to_response()
        {
            var endpointParticipant = ("sip123", LinkedParticipantType.DefenceAdvocate);
            var endpoint = new Endpoint("Display", "sip123", "1245", endpointParticipant);
            var response = EndpointToResponseMapper.MapEndpointResponse(endpoint);
            
            response.Id.Should().Be(endpoint.Id);
            response.Pin.Should().Be(endpoint.Pin);
            response.Status.Should().Be((EndpointState)endpoint.State);
            response.SipAddress.Should().Be(endpoint.SipAddress);
            response.EndpointParticipants.Should().ContainSingle(ep => ep.ParticipantUsername == endpointParticipant.Item1);
        }
    }
}
