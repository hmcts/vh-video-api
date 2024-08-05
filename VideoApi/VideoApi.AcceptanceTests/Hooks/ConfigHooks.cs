using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TechTalk.SpecFlow;
using Testing.Common.Configuration;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.Common.Configuration;
using VideoApi.Common.Security;
using VideoApi.Common.Security.Supplier.Kinly;
using VideoApi.Common.Security.Supplier.Vodafone;
using VideoApi.Contract.Responses;

namespace VideoApi.AcceptanceTests.Hooks
{
    [Binding]
    public class ConfigHooks
    {
        private readonly IConfigurationRoot _configRoot;

        public ConfigHooks(TestContext context)
        {
            _configRoot = ConfigRootBuilder.Build();
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
        }

        private static void RegisterDefaultData(TestContext context)
        {
            context.Test = new Test()
            {
                CaseName = "Video Api Automated Test",
                ConferenceIds = new List<Guid>(),
                ConferenceDetailsResponses = new List<ConferenceDetailsResponse>(),
            };
            context.Test.CaseName.Should().NotBeNullOrWhiteSpace();
        }

        private void RegisterHearingServices(TestContext context)
        {
            context.Config.Services = GetTargetTestEnvironment() == string.Empty ? Options.Create(_configRoot.GetSection("Services").Get<ServicesConfiguration>()).Value
                : Options.Create(_configRoot.GetSection($"Testing.{GetTargetTestEnvironment()}.Services").Get<ServicesConfiguration>()).Value;
            if (context.Config.Services == null && GetTargetTestEnvironment() != string.Empty)
                throw new InvalidOperationException(
                    $"Missing test secrets for running against: {GetTargetTestEnvironment()}");
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
            var azureConfig = context.Config.AzureAdConfiguration;
            context.Tokens.VideoApiBearerToken = await new AzureTokenProvider(Options.Create(azureConfig))
                .GetClientAccessToken(azureConfig.ClientId, azureConfig.ClientSecret,
                    context.Config.Services.VideoApiResourceId);
            context.Tokens.VideoApiBearerToken.Should().NotBeNullOrEmpty();
            
        }
    }
}
