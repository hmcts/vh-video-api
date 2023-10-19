using System;
using System.Diagnostics.CodeAnalysis;
using LaunchDarkly.Logging;
using LaunchDarkly.Sdk;
using LaunchDarkly.Sdk.Server;
using LaunchDarkly.Sdk.Server.Interfaces;

namespace VideoApi.Services
{
    public interface IFeatureToggles
    {
        public bool HrsIntegrationEnabled();
    }
    
    [ExcludeFromCodeCoverage]
    public class FeatureToggles : IFeatureToggles
    {
        private readonly ILdClient _ldClient;
        private readonly Context _context;
        private const string LdUser = "vh-video-api";
        private const string HrsIntegrationEnabledToggleKey = "hrs-integration";
    
        public FeatureToggles(string sdkKey, string environmentName)
        {
            var config = Configuration.Builder(sdkKey)
                .Logging(Components.Logging(Logs.ToWriter(Console.Out)).Level(LogLevel.Warn)).Build();
            _context = Context.Builder(LdUser).Name(environmentName).Build();
            Console.WriteLine($"FeatureToggles environment name: {environmentName}");
            _ldClient = new LdClient(config);
        }
    
        public bool HrsIntegrationEnabled()
        {
            if (!_ldClient.Initialized)
            {
                throw new InvalidOperationException("LaunchDarkly client not initialized");
            }
    
            return _ldClient.BoolVariation(HrsIntegrationEnabledToggleKey, _context);
        }
    }
}
