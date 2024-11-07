using TechTalk.SpecFlow;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.Contract.Responses;
using static Testing.Common.Helper.ApiUriFactory.SelfTestEndpoints;

namespace VideoApi.AcceptanceTests.Steps
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
            _context.Request = TestContext.Get(SelfTest);
        }

        [Then(@"the pexip service configuration should be retrieved")]
        public void ThenThePexipServiceConfigurationShouldBeRetrieved()
        {
            var pexipConfig = ApiRequestHelper.Deserialise<PexipConfigResponse>(_context.Response.Content);
            pexipConfig.PexipSelfTestNode.Should().NotBeNullOrWhiteSpace();
        }
    }
}
