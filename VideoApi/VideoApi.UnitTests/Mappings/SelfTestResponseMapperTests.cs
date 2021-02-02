using FluentAssertions;
using NUnit.Framework;
using VideoApi.Common.Configuration;
using VideoApi.Mappings;

namespace VideoApi.UnitTests.Mappings
{
    public class SelfTestResponseMapperTests
    {
        [Test]
        public void Should_map_all_properties()
        {
            var pexipConfig = new ServicesConfiguration();
            pexipConfig.PexipSelfTestNode = "self-test.node";

            var response = PexipConfigurationMapper.MapPexipConfigToResponse(pexipConfig);
            response.PexipSelfTestNode.Should().BeEquivalentTo(pexipConfig.PexipSelfTestNode);
        }
    }
}
