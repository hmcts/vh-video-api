using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using TechTalk.SpecFlow;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.Contract.Responses;
using static Testing.Common.Helper.ApiUriFactory.HealthCheckEndpoints;

namespace VideoApi.AcceptanceTests.Steps
{
    [Binding]
    public sealed class HealthCheckSteps 
    {
        private readonly TestContext _context;

        public HealthCheckSteps(TestContext injectedContext)
        {
            _context = injectedContext;
        }

        [Given(@"I have a get health request")]
        public void GivenIHaveAGetHealthRequest()
        {
            _context.Request = _context.Get(CheckServiceHealth);
        }

        [Then(@"the application version should be retrieved")]
        public void ThenTheApplicationVersionShouldBeRetrieved()
        {
            var model = RequestHelper.DeserialiseSnakeCaseJsonToResponse<HealthCheckResponse>(_context.Response.Content);
            model.Should().NotBeNull();
            model.AppVersion.Should().NotBeNull();
            model.AppVersion.FileVersion.Should().NotBeNull();
            model.AppVersion.InformationVersion.Should().NotBeNull();
        }
        
        [Then(@"the Wowza health should be retrieved")]
        public void ThenTheWowzaHealthShouldBeRetrieved()
        {
            var model = RequestHelper.DeserialiseSnakeCaseJsonToResponse<HealthCheckResponse>(_context.Response.Content);
            model.Should().NotBeNull();
            model.WowzaHealth.Should().NotBeNull();
            model.WowzaHealth.Successful.Should().BeTrue();
        }
    }
}
