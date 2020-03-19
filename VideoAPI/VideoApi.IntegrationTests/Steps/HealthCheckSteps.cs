using FluentAssertions;
using System.Net.Http;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using TechTalk.SpecFlow;
using VideoApi.Contract.Responses;
using VideoApi.IntegrationTests.Contexts;
using static Testing.Common.Helper.ApiUriFactory.HealthCheckEndpoints;

namespace VideoApi.IntegrationTests.Steps
{
    [Binding]
    public sealed class HealthCheckSteps : BaseSteps
    {
        private readonly TestContext _context;

        public HealthCheckSteps(TestContext context)
        {
            _context = context;
        }

        [Given(@"I have a get health request")]
        public void GivenIMakeACallToTheHealthCheckEndpoint()
        {
            _context.Uri = CheckServiceHealth;
            _context.HttpMethod = HttpMethod.Get;
        }

        [Then(@"the application version should be retrieved")]
        public async Task ThenTheApplicationVersionShouldBeRetrieved()
        {
            var json = await _context.ResponseMessage.Content.ReadAsStringAsync();
            var getResponseModel = RequestHelper.DeserialiseSnakeCaseJsonToResponse<HealthCheckResponse>(json);
            getResponseModel.AppVersion.Should().NotBeNull();
            getResponseModel.AppVersion.FileVersion.Should().NotBeNull();
            getResponseModel.AppVersion.InformationVersion.Should().NotBeNull();
        }
    }
}
