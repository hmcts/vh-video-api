using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using TechTalk.SpecFlow;
using Testing.Common.Configuration;
using Video.API;
using VideoApi.Common.Configuration;
using VideoApi.Common.Security;
using VideoApi.DAL;
using VideoApi.IntegrationTests.Contexts;
using VideoApi.IntegrationTests.Helper;

namespace VideoApi.IntegrationTests.Hooks
{
    [Binding]
    public class ApiHooks
    {
        protected ApiHooks()
        {
        }

        [BeforeFeature]
        public static void BeforeApiFeature(ApiTestContext apiTestContext)
        {
            var webHostBuilder = WebHost.CreateDefaultBuilder()
                .UseKestrel(c => c.AddServerHeader = false)
                .UseEnvironment("Development")
                .UseStartup<Startup>();
            apiTestContext.Server = new TestServer(webHostBuilder);
            GetClientAccessTokenForApi(apiTestContext);

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddUserSecrets<Startup>().Build();

            apiTestContext.DbString = configuration.GetConnectionString("VhVideoApi");
            apiTestContext.Services =
                Options.Create(configuration.GetSection("Services").Get<ServicesConfiguration>()).Value;

            var dbContextOptionsBuilder = new DbContextOptionsBuilder<VideoApiDbContext>();
            dbContextOptionsBuilder.EnableSensitiveDataLogging();
            dbContextOptionsBuilder.UseSqlServer(apiTestContext.DbString);
            apiTestContext.VideoBookingsDbContextOptions = dbContextOptionsBuilder.Options;
            apiTestContext.TestDataManager =
                new TestDataManager(apiTestContext.Services, apiTestContext.VideoBookingsDbContextOptions);
        }

        private static void GetClientAccessTokenForApi(ApiTestContext apiTestContext)
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
            var testSettings = testSettingsOptions.Value;

            apiTestContext.BearerToken = new AzureTokenProvider(azureAdConfigurationOptions).GetClientAccessToken(
                testSettings.TestClientId, testSettings.TestClientSecret,
                azureAdConfiguration.VhVideoApiResourceId);
        }

        [BeforeScenario]
        public static void BeforeApiScenario(ApiTestContext apiTestContext)
        {
            apiTestContext.NewConferenceId = Guid.Empty;
        }

        [AfterScenario]
        public static async Task AfterApiScenario(ApiTestContext apiTestContext,
            ConferenceTestContext conferenceTestContext)
        {
            if (apiTestContext.NewConferenceId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test hearing {apiTestContext.NewConferenceId}");
                await apiTestContext.TestDataManager.RemoveConference(apiTestContext.NewConferenceId);
            }
            await apiTestContext.TestDataManager.RemoveConferences(conferenceTestContext.SeededConferences);
        }

        [AfterFeature]
        public static async Task AfterApiFeature(ApiTestContext apiTestContext)
        {
            await apiTestContext.TestDataManager.RemoveEvents();
            apiTestContext.Server.Dispose();
        }
    }
}