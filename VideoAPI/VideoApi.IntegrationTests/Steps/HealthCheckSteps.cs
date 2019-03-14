using System.Net.Http;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using VideoApi.IntegrationTests.Contexts;

namespace VideoApi.IntegrationTests.Steps
{
    [Binding]
    public sealed class HealthCheckSteps : StepsBase
    {
        private readonly HealthCheckEndpoints _endpoints = new ApiUriFactory().HealthCheckEndpoints;

        public HealthCheckSteps(ApiTestContext apiTestContext) : base(apiTestContext)
        {
        }

        [Given(@"I have a get health request")]
        public void GivenIMakeACallToTheHealthCheckEndpoint()
        {
            ApiTestContext.Uri = _endpoints.CheckServiceHealth();
            ApiTestContext.HttpMethod = HttpMethod.Get;
        }
    }
}