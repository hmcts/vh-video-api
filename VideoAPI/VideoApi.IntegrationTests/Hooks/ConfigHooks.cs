using System.Collections.Generic;
using System.Net.Http;
using AcceptanceTests.Common.Configuration;
using FluentAssertions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TechTalk.SpecFlow;
using Testing.Common.Configuration;
using Video.API;
using VideoApi.Common.Configuration;
using VideoApi.Common.Security;
using VideoApi.DAL;
using VideoApi.Domain;
using VideoApi.IntegrationTests.Contexts;
using VideoApi.IntegrationTests.Helper;

namespace VideoApi.IntegrationTests.Hooks
{
    [Binding]
    public class ConfigHooks
    {
        private readonly IConfigurationRoot _configRoot;

        public ConfigHooks(TestContext context)
        {
            _configRoot = ConfigurationManager.BuildConfig("9AECE566-336D-4D16-88FA-7A76C27321CD");
            context.Config = new Config();
            context.Tokens = new VideoApiTokens();
        }

        [BeforeScenario(Order = (int)HooksSequence.ConfigHooks)]
        public void RegisterSecrets(TestContext context)
        {
            var azureOptions = RegisterAzureSecrets(context);
            RegisterDefaultData(context);
            RegisterHearingServices(context);
            RegisterDatabaseSettings(context);
            RegisterServer(context);
            RegisterZapSettings(context);
            RegisterApiSettings(context);
            GenerateBearerTokens(context, azureOptions);
        }

        private IOptions<AzureAdConfiguration> RegisterAzureSecrets(TestContext context)
        {
            var azureOptions = Options.Create(_configRoot.GetSection("AzureAd").Get<AzureAdConfiguration>());
            context.Config.AzureAdConfiguration = azureOptions.Value;
            ConfigurationManager.VerifyConfigValuesSet(context.Config.AzureAdConfiguration);
            return azureOptions;
        }

        private static void RegisterDefaultData(TestContext context)
        {
            context.Test = new Test()
            {
                CaseName = "Video Api Integration Test",
                ClosedConferences = new List<Conference>(),
                ClosedConferencesWithMessages = new List<Conference>(),
                Conferences = new List<Conference>(),
                TodaysConferences = new List<Conference>()
            };
            context.Test.CaseName.Should().NotBeNullOrWhiteSpace();
        }

        private void RegisterHearingServices(TestContext context)
        {
            context.Config.VhServices = Options.Create(_configRoot.GetSection("Services").Get<ServicesConfiguration>()).Value;
            ConfigurationManager.VerifyConfigValuesSet(context.Config.VhServices);
        }

        private void RegisterDatabaseSettings(TestContext context)
        {
            context.Config.DbConnection = Options.Create(_configRoot.GetSection("ConnectionStrings").Get<ConnectionStringsConfig>()).Value;
            ConfigurationManager.VerifyConfigValuesSet(context.Config.DbConnection);
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<VideoApiDbContext>();
            dbContextOptionsBuilder.EnableSensitiveDataLogging();
            dbContextOptionsBuilder.UseSqlServer(context.Config.DbConnection.VhVideoApi);
            context.Config.VideoBookingsDbContextOptions = dbContextOptionsBuilder.Options;
            context.TestDataManager = new TestDataManager(context.Config.VhServices, context.Config.VideoBookingsDbContextOptions);
        }

        private static void RegisterServer(TestContext context)
        {
            var webHostBuilder = WebHost.CreateDefaultBuilder()
                    .UseKestrel(c => c.AddServerHeader = false)
                    .UseEnvironment("Development")
                    .UseStartup<Startup>();
            context.Server = new TestServer(webHostBuilder);
        }

        private void RegisterZapSettings(TestContext context)
        {
            context.Config.ZapConfig = Options.Create(_configRoot.GetSection("ZapConfiguration").Get<ZapConfiguration>()).Value;
        }

        private static void RegisterApiSettings(TestContext context)
        {
            context.Response = new HttpResponseMessage(); 
        }

        private static void GenerateBearerTokens(TestContext context, IOptions<AzureAdConfiguration> azureOptions)
        {
            context.Tokens.VideoApiBearerToken = new AzureTokenProvider(azureOptions).GetClientAccessToken(
                azureOptions.Value.ClientId, azureOptions.Value.ClientSecret,
                context.Config.VhServices.VhVideoApiResourceId);
            context.Tokens.VideoApiBearerToken.Should().NotBeNullOrEmpty();
        }
    }
}

