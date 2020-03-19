using System;
using System.Linq;
using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using Video.API;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.AcceptanceTests.Helpers;
using VideoApi.Common.Configuration;
using VideoApi.Common.Security.CustomToken;
using static Testing.Common.Helper.ApiUriFactory.HealthCheckEndpoints;

namespace VideoApi.AcceptanceTests.Hooks
{
    [Binding]
    public static class ApiHooks
    {
        [BeforeTestRun]
        public static void OneTimeSetup(TestContext context)
        {
            Zap.StartZapDaemon();

            var configRootBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddUserSecrets<Startup>();

            var configRoot = configRootBuilder.Build();

            var azureAdConfigurationOptions = Options.Create(configRoot.GetSection("AzureAd").Get<AzureAdConfiguration>());
            var serviceSettingsOptions = Options.Create(configRoot.GetSection("Services").Get<ServicesConfiguration>());

            context.CustomTokenSettings = configRoot.GetSection("CustomToken").Get<CustomTokenSettings>();
            context.AzureAdConfiguration = azureAdConfigurationOptions;
            context.ServicesConfiguration = serviceSettingsOptions.Value;

            context.SetDefaultBearerToken();

            var apiTestsOptions =
                Options.Create(configRoot.GetSection("AcceptanceTestSettings").Get<TestConfiguration>());
            var apiTestSettings = apiTestsOptions.Value;
            context.BaseUrl = apiTestSettings.VideoApiBaseUrl;
            context.BaseUrl.Should().NotBeNullOrEmpty();

            VerifyCustomSecretsAreSet(context.CustomTokenSettings);
            VerifyAzureSecretsAreSet(context.AzureAdConfiguration.Value);
            VerifyServicesSecretsAreSet(context.ServicesConfiguration);
        }

        private static void VerifyCustomSecretsAreSet(CustomTokenSettings customTokenSettings)
        {
            customTokenSettings.Secret.Should().NotBeNullOrEmpty();
            customTokenSettings.ThirdPartySecret.Should().NotBeNullOrEmpty();
        }

        private static void VerifyAzureSecretsAreSet(AzureAdConfiguration azureAdConfiguration)
        {
            azureAdConfiguration.Authority.Should().NotBeNullOrEmpty();
            azureAdConfiguration.ClientId.Should().NotBeNullOrEmpty();
            azureAdConfiguration.ClientSecret.Should().NotBeNullOrEmpty();
            azureAdConfiguration.TenantId.Should().NotBeNullOrEmpty();
        }

        private static void VerifyServicesSecretsAreSet(ServicesConfiguration servicesConfiguration)
        {
            servicesConfiguration.CallbackUri.Should().NotBeNullOrEmpty();
            servicesConfiguration.ConferenceUsername.Should().NotBeNullOrEmpty();
            servicesConfiguration.KinlyApiUrl.Should().NotBeNullOrEmpty();
            servicesConfiguration.KinlySelfTestApiUrl.Should().NotBeNullOrEmpty();
            servicesConfiguration.PexipNode.Should().NotBeNullOrEmpty();
            servicesConfiguration.PexipSelfTestNode.Should().NotBeNullOrEmpty();
            servicesConfiguration.UserApiResourceId.Should().NotBeNullOrEmpty();
            servicesConfiguration.UserApiUrl.Should().NotBeNullOrEmpty();
            servicesConfiguration.VhVideoApiResourceId.Should().NotBeNullOrEmpty();
            servicesConfiguration.VhVideoWebClientId.Should().NotBeNullOrEmpty();
        }

        [BeforeTestRun]
        public static void CheckHealth(TestContext context)
        {
            context.Request = context.Get(CheckServiceHealth);
            context.Response = context.Client().Execute(context.Request);
            context.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [AfterScenario]
        public static void RemoveConference(TestContext context)
        {
            if (context.NewConferenceId == Guid.Empty) return;
            RemoveConference(context, context.NewConferenceId);
        }

        [AfterScenario]
        public static void RemoveConferences(TestContext context)
        {
            if (context.NewConferenceIds.Count <= 0) return;
            foreach (var id in context.NewConferenceIds.Where(id => !id.Equals(context.NewConferenceId)))
                RemoveConference(context, id);
            context.NewConferences.Clear();
            context.NewConferenceIds.Clear();
            context.NewConferenceId = Guid.Empty;
        }

        private static void RemoveConference(TestContext context, Guid conferenceId)
        {
            context.SetDefaultBearerToken();
            context.Request = context.Delete(ApiUriFactory.ConferenceEndpoints.RemoveConference(conferenceId));
            context.Response = context.Client().Execute(context.Request);
            context.Response.IsSuccessful.Should().BeTrue("Conference is deleted");
        }
    }
}
