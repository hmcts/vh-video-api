using FluentAssertions;
using NUnit.Framework;
using Video.API.Mappings;
using VideoApi.Common.Configuration;

namespace VideoApi.UnitTests.Mappings
{
    public class SelfTestResponseMapperTests
    {
        private readonly PexipConfigurationMapper _mapper = new PexipConfigurationMapper();

        [Test]
        public void Should_map_all_properties()
        {
            var pexipConfig = new ServicesConfiguration();
            pexipConfig.PexipSelfTestNode = "self-test.node";

            var response = _mapper.MapPexipConfigToResponse(pexipConfig);
            response.PexipSelfTestNode.Should().BeEquivalentTo(pexipConfig.PexipSelfTestNode);
        }
    }
}
