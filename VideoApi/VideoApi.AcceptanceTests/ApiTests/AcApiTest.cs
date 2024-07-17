using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Testing.Common.Configuration;
using VideoApi.Client;
using VideoApi.Common.Configuration;
using VideoApi.Common.Security;
using VideoApi.Common.Security.Supplier.Kinly;
using VideoApi.Common.Security.Supplier.Vodafone;
using VideoApi.Services;

namespace VideoApi.AcceptanceTests.ApiTests;

public abstract class AcApiTest
{
    private IConfigurationRoot _configRoot;
    private AzureAdConfiguration _azureConfiguration;
    private ServicesConfiguration _serviceConfiguration;
    private KinlyConfiguration _kinlyConfiguration;
    private VodafoneConfiguration _vodafoneConfiguration;
    
    protected WowzaConfiguration WowzaConfiguration;
    protected IFeatureToggles FeatureToggles;
    
    protected VideoApiClient VideoApiClient { get; set; }
    
    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        RegisterSettings();
        InitFeatureToggle();
        await InitApiClients();
    }
    
    protected string GetSupplierSipAddressStem()
    {
        var sipStem = FeatureToggles.VodafoneIntegrationEnabled()
            ? _vodafoneConfiguration.SipAddressStem
            : _kinlyConfiguration.SipAddressStem;
        return sipStem;
    }
    
    private void RegisterSettings()
    {
        _configRoot = ConfigRootBuilder.Build();
        _azureConfiguration = _configRoot.GetSection("AzureAd").Get<AzureAdConfiguration>();
        _serviceConfiguration = _configRoot.GetSection("Services").Get<ServicesConfiguration>();
        _kinlyConfiguration = _configRoot.GetSection("KinlyConfiguration").Get<KinlyConfiguration>();
        _vodafoneConfiguration = _configRoot.GetSection("VodafoneConfiguration").Get<VodafoneConfiguration>();
        WowzaConfiguration =  _configRoot.GetSection("WowzaConfiguration").Get<WowzaConfiguration>();
    }
    
    private async Task InitApiClients()
    {
        var apiToken = await GenerateApiToken();
        var notificationApiHttpClient = new HttpClient();
        notificationApiHttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("bearer", apiToken);
        VideoApiClient = VideoApiClient.GetClient(_serviceConfiguration.VideoApiUrl, notificationApiHttpClient);
    }
    
    private void InitFeatureToggle()
    {
        var envName = _serviceConfiguration.VideoApiUrl;
        FeatureToggles = new FeatureToggles(_configRoot["LaunchDarkly:SdkKey"], envName);
    }
    
    private async Task<string> GenerateApiToken()
    {
        return await new AzureTokenProvider(Options.Create(_azureConfiguration)).GetClientAccessToken(_azureConfiguration.ClientId,
            _azureConfiguration.ClientSecret, _serviceConfiguration.VideoApiResourceId);
    }
}
