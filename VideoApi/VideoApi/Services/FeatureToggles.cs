using System;
using LaunchDarkly.Logging;
using LaunchDarkly.Sdk;
using LaunchDarkly.Sdk.Server;
using LaunchDarkly.Sdk.Server.Interfaces;
namespace VideoApi.Services
{
    public interface IFeatureToggles
    {
        public bool HrsIntegrationToggle();
    }
    
    public class FeatureToggles : IFeatureToggles
    {
        private readonly ILdClient _ldClient;
        private readonly User _user;
        private const string HrsIntegration="hrs-integration";
        private const string LdUser = "vh-video-api";
   
        public FeatureToggles(string sdkKey)
        {
            var config = Configuration.Builder(sdkKey)
                .Logging(
                    Components.Logging(Logs.ToWriter(Console.Out)).Level(LogLevel.Warn)
                )
                .Build();
            _ldClient = new LdClient(config);
            _user = User.WithKey(LdUser);
        }
        public bool HrsIntegrationToggle() => _ldClient.BoolVariation(HrsIntegration, _user);
    }
}
