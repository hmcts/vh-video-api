using System;
using System.Diagnostics.CodeAnalysis;
using LaunchDarkly.Logging;
using LaunchDarkly.Sdk;
using LaunchDarkly.Sdk.Server;

namespace VideoApi.Services;

public interface IFeatureToggles
{
    public bool VodafoneIntegrationEnabled();
    public bool SendTransferRolesEnabled();
}

[ExcludeFromCodeCoverage]
public class FeatureToggles : IFeatureToggles
{
    private const string LdUser = "vh-video-api";
    private const string VodafoneToggleKey = "vodafone";
    private const string SendTransferRolesKey = "send-transfer-roles";
    private readonly Context _context;
    private readonly LdClient _ldClient;
    
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
    
    public bool SendTransferRolesEnabled()
    {
        if (!_ldClient.Initialized)
        {
            throw new InvalidOperationException("LaunchDarkly client not initialized");
        }
        
        return _ldClient.BoolVariation(SendTransferRolesKey, _context);
    }
}
