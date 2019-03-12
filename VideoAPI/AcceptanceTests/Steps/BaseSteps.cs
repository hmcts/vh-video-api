using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TechTalk.SpecFlow;
using Testing.Common.Configuration;
using Testing.Common.Helper;
using Video.API;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.AcceptanceTests.Helpers;
using VideoApi.Common.Configuration;
using VideoApi.Common.Security;

namespace VideoApi.AcceptanceTests.Steps
{
    [Binding]
    public abstract class BaseSteps
    {
        protected BaseSteps()
        {
        }

        [BeforeTestRun]
        public static void OneTimeSetup(TestContext context)
        {
            var configRootBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddUserSecrets<Startup>();

            var configRoot = configRootBuilder.Build();

            var azureAdConfigurationOptions =
                Options.Create(configRoot.GetSection("AzureAd").Get<AzureAdConfiguration>());
            var testSettingsOptions = Options.Create(configRoot.GetSection("Testing").Get<TestSettings>());

            var azureAdConfiguration = azureAdConfigurationOptions.Value;
            context.TestSettings = testSettingsOptions.Value;

            context.BearerToken = new AzureTokenProvider(azureAdConfigurationOptions).GetClientAccessToken(
                context.TestSettings.TestClientId, context.TestSettings.TestClientSecret,
                azureAdConfiguration.VhVideoApiResourceId);

            var apiTestsOptions =
                Options.Create(configRoot.GetSection("AcceptanceTestSettings").Get<TestConfiguration>());
            var apiTestSettings = apiTestsOptions.Value;
            context.BaseUrl = apiTestSettings.VideoApiBaseUrl;
        }

        [BeforeTestRun]
        public static void CheckHealth(TestContext context)
        {
            //var endpoint = new ApiUriFactory().HealthCheckEndpoints;
            //context.Request = context.Get(endpoint.CheckServiceHealth());
            //context.Response = context.Client().Execute(context.Request);
            //context.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
