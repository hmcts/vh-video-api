using System;
using System.Diagnostics.CodeAnalysis;
using LaunchDarkly.Logging;
using LaunchDarkly.Sdk;
using LaunchDarkly.Sdk.Server;

namespace VideoApi.Services
{
    public interface IFeatureToggles
    {
        public bool VodafoneIntegrationEnabled();
    }
    
    [ExcludeFromCodeCoverage]
    public class FeatureToggles : IFeatureToggles
    {
        private readonly LdClient _ldClient;
        private readonly Context _context;
        private const string LdUser = "vh-video-api";
        private const string VodafoneToggleKey = "vodafone";
    
        public FeatureToggles(string sdkKey, string environmentName)
        {
            var config = Configuration.Builder(sdkKey)
                .Logging(Components.Logging(Logs.ToWriter(Console.Out)).Level(LogLevel.Warn)).Build();
            _context = Context.Builder(LdUser).Name(environmentName).Build();
            _ldClient = new LdClient(config);
        }
        
        public bool VodafoneIntegrationEnabled()
        {
            if (!_ldClient.Initialized)
            {
                throw new InvalidOperationException("LaunchDarkly client not initialized");
            }
    
            return _ldClient.BoolVariation(VodafoneToggleKey, _context);
        }
    }
}
