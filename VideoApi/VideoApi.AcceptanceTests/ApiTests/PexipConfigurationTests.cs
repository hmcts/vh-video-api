using System.Threading.Tasks;
using NUnit.Framework;

namespace VideoApi.AcceptanceTests.ApiTests;

public class PexipConfigurationTests : AcApiTest
{
    [Test]
    public async Task should_return_pexip_config_with_self_test()
    {
        var pexipConfiguration = await VideoApiClient.GetPexipServicesConfigurationAsync();
        pexipConfiguration.Should().NotBeNull();
        pexipConfiguration.PexipSelfTestNode.Should().NotBeNullOrWhiteSpace();
    }
}
