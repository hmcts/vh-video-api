using FluentAssertions;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using VideoApi.Common.Helpers;
using VideoApi.Contract.Responses;
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

        [Then(@"the application version should be retrieved")]
        public async Task ThenTheApplicationVersionShouldBeRetrieved()
        {
            var json = await ApiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var getResponseModel = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<HealthCheckResponse>(json);
            getResponseModel.AppVersion.Should().NotBeNull();
            getResponseModel.AppVersion.FileVersion.Should().NotBeNull();
            getResponseModel.AppVersion.InformationVersion.Should().NotBeNull();
        }
    }
}
