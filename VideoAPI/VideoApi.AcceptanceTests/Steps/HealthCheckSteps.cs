using FluentAssertions;
using System;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.Common.Helpers;
using VideoApi.Contract.Responses;

namespace VideoApi.AcceptanceTests.Steps
{
    [Binding]
    public sealed class HealthCheckSteps : BaseSteps
    {
        private readonly TestContext _context;
        private readonly HealthCheckEndpoints _endpoints = new ApiUriFactory().HealthCheckEndpoints;

        public HealthCheckSteps(TestContext injectedContext)
        {
            _context = injectedContext;
        }

        [Given(@"I have a get health request")]
        public void GivenIHaveAGetHealthRequest()
        {
            _context.NewConferenceId = Guid.Empty;
            _context.Request = _context.Get(_endpoints.CheckServiceHealth());
        }

        [Then(@"the application version should be retrieved")]
        public void ThenTheApplicationVersionShouldBeRetrieved()
        {
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<HealthCheckResponse>(_context.Json);
            model.Should().NotBeNull();
            model.AppVersion.Should().NotBeNull();
            model.AppVersion.FileVersion.Should().NotBeNull();
            model.AppVersion.InformationVersion.Should().NotBeNull();
        }
    }
}
