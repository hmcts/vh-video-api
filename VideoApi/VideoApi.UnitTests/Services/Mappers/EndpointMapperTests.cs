using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services.Mappers;

namespace VideoApi.UnitTests.Services.Mappers
{
    public class EndpointMapperTests
    {
        [Test]
        public void should_map_to_kinly_endpoint_dto()
        {
            var ep = new Endpoint("Displayname", "sip", "pin", ("defence_advocate@hmcts.net", LinkedParticipantType.DefenceAdvocate));
            var dto = EndpointMapper.MapToEndpoint(ep);

            dto.Id.Should().Be(ep.Id);
            dto.Pin.Should().Be(ep.Pin);
            dto.DisplayName.Should().Be(ep.DisplayName);
            dto.SipAddress.Should().Be(ep.SipAddress);
        }
    }
}
