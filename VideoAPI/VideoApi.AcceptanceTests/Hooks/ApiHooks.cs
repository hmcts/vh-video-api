using System;
using System.Linq;
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
using VideoApi.Common.Security.CustomToken;

namespace VideoApi.AcceptanceTests.Hooks
{
    [Binding]
    public static class ApiHooks
    {
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
            var serviceSettingsOptions = Options.Create(configRoot.GetSection("Services").Get<ServicesConfiguration>());
            context.CustomTokenSettings = configRoot.GetSection("CustomToken").Get<CustomTokenSettings>();

            context.AzureAdConfiguration = azureAdConfigurationOptions;
            context.TestSettings = testSettingsOptions.Value;
            context.ServicesConfiguration = serviceSettingsOptions.Value;

            context.SetDefaultBearerToken();

            var apiTestsOptions =
                Options.Create(configRoot.GetSection("AcceptanceTestSettings").Get<TestConfiguration>());
            var apiTestSettings = apiTestsOptions.Value;
            context.BaseUrl = apiTestSettings.VideoApiBaseUrl;
        }

        [BeforeTestRun]
        public static void CheckHealth(TestContext context)
        {
            var endpoint = new ApiUriFactory().HealthCheckEndpoints;
            context.Request = context.Get(endpoint.CheckServiceHealth());
            context.Response = context.Client().Execute(context.Request);
            context.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [AfterScenario]
        public static void RemoveConference(TestContext context, ConferenceEndpoints endpoints)
        {
            if (context.NewConferenceId == Guid.Empty) return;
            RemoveConference(context, endpoints, context.NewConferenceId);
        }

        [AfterScenario]
        public static void RemoveConferences(TestContext context, ConferenceEndpoints endpoints)
        {
            if (context.NewConferenceIds.Count <= 0) return;
            foreach (var id in context.NewConferenceIds.Where(id => !id.Equals(context.NewConferenceId)))
            {
                RemoveConference(context, endpoints, id);
            }
            context.NewConferences.Clear();
            context.NewConferenceIds.Clear();
            context.NewConferenceId = Guid.Empty;
        }

        private static void RemoveConference(TestContext context, ConferenceEndpoints endpoints, Guid conferenceId)
        {
            context.SetDefaultBearerToken();
            context.Request = context.Delete(endpoints.RemoveConference(conferenceId));
            context.Response = context.Client().Execute(context.Request);
            context.Response.IsSuccessful.Should().BeTrue("Conference is deleted");
        }
    }
}
