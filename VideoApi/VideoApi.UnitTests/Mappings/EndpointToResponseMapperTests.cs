using Testing.Common.Helper.Builders.Domain;
using VideoApi.Contract.Enums;
using VideoApi.Domain;
using VideoApi.Mappings;
using ConferenceRole = VideoApi.Domain.Enums.ConferenceRole;

namespace VideoApi.UnitTests.Mappings
{
    public class EndpointToResponseMapperTests
    {
        [Test]
        public void should_map_endpoint_to_response()
        {
            var endpoint = new Endpoint("Display", "sip123", "1245", ConferenceRole.Guest);
            var da = new ParticipantBuilder().Build();
            endpoint.AddParticipantLink(da);
            var response = EndpointToResponseMapper.MapEndpointResponse(endpoint);
            
            response.Id.Should().Be(endpoint.Id);
            response.Pin.Should().Be(endpoint.Pin);
            response.Status.Should().Be((EndpointState)endpoint.State);
            response.SipAddress.Should().Be(endpoint.SipAddress);
            response.ParticipantsLinked.Should().NotBeEmpty().And.Contain(da.Username);
            response.ConferenceRole.Should().Be((Contract.Enums.ConferenceRole)endpoint.ConferenceRole);
        }
    }
}
