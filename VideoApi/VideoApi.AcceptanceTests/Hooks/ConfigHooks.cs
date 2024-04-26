using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api;
using AcceptanceTests.Common.Configuration;
using AcceptanceTests.Common.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TechTalk.SpecFlow;
using Testing.Common.Configuration;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.Common.Configuration;
using VideoApi.Common.Security.Supplier.Kinly;
using VideoApi.Common.Security.Supplier.Vodafone;
using VideoApi.Contract.Responses;
using ConfigurationManager = AcceptanceTests.Common.Configuration.ConfigurationManager;

namespace VideoApi.AcceptanceTests.Hooks
{
    [Binding]
    public class ConfigHooks
    {
        private readonly IConfigurationRoot _configRoot;

        public ConfigHooks(TestContext context)
        {
            _configRoot = ConfigurationManager.BuildConfig("9AECE566-336D-4D16-88FA-7A76C27321CD", "6b1d4c67-28f8-4104-8424-9379113b6631");
            context.Config = new Config();
            context.Tokens = new VideoApiTokens();
        }

        [BeforeScenario(Order = (int)HooksSequence.ConfigHooks)]
        public async Task RegisterSecrets(TestContext context)
        {
            RegisterAzureSecrets(context);
            RegisterDefaultData(context);
            RegisterHearingServices(context);
            RegisterKinlySettings(context);
            RegisterVodafoneSettings(context);
            RegisterWowzaSettings(context);
            RegisterAudioRecordingTestIdConfiguration(context);
            await GenerateBearerTokens(context);
        }

        private void RegisterAzureSecrets(TestContext context)
        {
            context.Config.AzureAdConfiguration = Options.Create(_configRoot.GetSection("AzureAd").Get<AzureAdConfiguration>()).Value;
            context.Config.AzureAdConfiguration.Authority += context.Config.AzureAdConfiguration.TenantId;
            ConfigurationManager.VerifyConfigValuesSet(context.Config.AzureAdConfiguration);
        }

        private static void RegisterDefaultData(TestContext context)
        {
            context.Test = new Test()
            {
                CaseName = "Video Api Automated Test",
                ConferenceIds = new List<Guid>(),
                ConferenceResponses = new List<ConferenceForAdminResponse>(),
                ConferenceDetailsResponses = new List<ConferenceDetailsResponse>(),
                ConferenceJudgeResponses = new List<ConferenceForHostResponse>(),
                ConferenceIndividualResponses = new List<ConferenceForIndividualResponse>()
            };
            context.Test.CaseName.Should().NotBeNullOrWhiteSpace();
        }

        private void RegisterHearingServices(TestContext context)
        {
            context.Config.Services = GetTargetTestEnvironment() == string.Empty ? Options.Create(_configRoot.GetSection("Services").Get<ServicesConfiguration>()).Value
                : Options.Create(_configRoot.GetSection($"Testing.{GetTargetTestEnvironment()}.Services").Get<ServicesConfiguration>()).Value;
            if (context.Config.Services == null && GetTargetTestEnvironment() != string.Empty) throw new TestSecretsFileMissingException(GetTargetTestEnvironment());
            ConfigurationManager.VerifyConfigValuesSet(context.Config.Services);
        }
        
        private void RegisterKinlySettings(TestContext context)
        {
            context.Config.KinlyConfiguration = Options.Create(_configRoot.GetSection("KinlyConfiguration").Get<KinlyConfiguration>()).Value;
            context.Config.KinlyConfiguration.CallbackUri = context.Config.Services.CallbackUri;
            context.Config.KinlyConfiguration.CallbackUri.Should().NotBeEmpty();
            context.Config.KinlyConfiguration.ApiUrl.Should().NotBeEmpty();
        }        
        
        private void RegisterVodafoneSettings(TestContext context)
        {
            context.Config.VodafoneConfiguration = Options.Create(_configRoot.GetSection("VodafoneConfiguration").Get<VodafoneConfiguration>()).Value;
            context.Config.VodafoneConfiguration.CallbackUri = context.Config.Services.CallbackUri;
            context.Config.VodafoneConfiguration.CallbackUri.Should().NotBeEmpty();
            context.Config.VodafoneConfiguration.ApiUrl.Should().NotBeEmpty();
        }

        private void RegisterWowzaSettings(TestContext context)
        {
            context.Config.Wowza = Options.Create(_configRoot.GetSection("WowzaConfiguration").Get<WowzaConfiguration>()).Value;
            context.Config.Wowza.StorageAccountKey.Should().NotBeNullOrEmpty();
            context.Config.Wowza.StorageAccountName.Should().NotBeNullOrEmpty(); 
            context.Config.Wowza.StorageContainerName.Should().NotBeNullOrEmpty();
        }

        private void RegisterAudioRecordingTestIdConfiguration(TestContext context)
        {
            context.Config.AudioRecordingTestIds = new AudioRecordingTestIdConfiguration();
            context.Config.AudioRecordingTestIds.NonExistent.Should().NotBeEmpty();
            context.Config.AudioRecordingTestIds.Existing.Should().NotBeEmpty();
        }

        private static string GetTargetTestEnvironment()
        {
            return NUnit.Framework.TestContext.Parameters["TargetTestEnvironment"] ?? String.Empty;
        }

        private static async Task GenerateBearerTokens(TestContext context)
        {
            var azureConfig = new AzureAdConfig()
            {
                Authority = context.Config.AzureAdConfiguration.Authority,
                ClientId = context.Config.AzureAdConfiguration.ClientId,
                ClientSecret = context.Config.AzureAdConfiguration.ClientSecret,
                TenantId = context.Config.AzureAdConfiguration.TenantId
            };

            context.Tokens.VideoApiBearerToken = await ConfigurationManager.GetBearerToken(
                azureConfig, context.Config.Services.VideoApiResourceId);
            context.Tokens.VideoApiBearerToken.Should().NotBeNullOrEmpty();
            
            Zap.SetAuthToken(context.Tokens.VideoApiBearerToken);
        }
    }

    internal class AzureAdConfig : IAzureAdConfig
    {
        public string Authority { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string TenantId { get; set; }
    }
}
