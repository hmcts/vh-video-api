using VideoApi.Common.Security.Supplier.Vodafone;
using VideoApi.Mappings;

namespace VideoApi.UnitTests.Mappings
{
    public class SelfTestResponseMapperTests
    {
        [Test]
        public void Should_map_all_properties()
        {
            var pexipConfig = new VodafoneConfiguration
            {
                PexipSelfTestNode = "self-test.node"
            };

            var response = PexipConfigurationMapper.MapPexipConfigToResponse(pexipConfig);
            response.PexipSelfTestNode.Should().BeEquivalentTo(pexipConfig.PexipSelfTestNode);
        }
    }
}
