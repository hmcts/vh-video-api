using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using TechTalk.SpecFlow;
using VideoApi.Contract.Responses;
using VideoApi.IntegrationTests.Contexts;
using VideoApi.IntegrationTests.Helper;
using static Testing.Common.Helper.ApiUriFactory.SelfTestEndpoints;

namespace VideoApi.IntegrationTests.Steps
{
    [Binding]
    public class SelfTestSteps
    {
        private readonly TestContext _context;
        public SelfTestSteps(TestContext context)
        {
            _context = context;
        }

        [Given(@"I have a self test request")]
        public void GivenIHaveASelfTestRequest()
        {
            _context.Uri = SelfTest;
            _context.HttpMethod = HttpMethod.Get;
        }

        [Then(@"the pexip service configuration should be retrieved")]
        public async Task ThenThePexipServiceConfigurationShouldBeRetrieved()
        {
            var pexipConfig = await ApiClientResponse.GetResponses<PexipConfigResponse>(_context.Response.Content);
            pexipConfig.PexipSelfTestNode.Should().NotBeNullOrWhiteSpace();
        }
    }
}
