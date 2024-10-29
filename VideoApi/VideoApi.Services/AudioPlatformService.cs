using System;
using System.Text.RegularExpressions;
using VideoApi.Common.Configuration;
using VideoApi.Services.Contracts;

namespace VideoApi.Services;

public class AudioPlatformService(WowzaConfiguration configuration) : IAudioPlatformService
{
    public string GetAudioIngestUrl(string serviceId, string caseNumber, string hearingId)
    {
        const string regex = "[^a-zA-Z0-9]";
        const RegexOptions regexOptions = RegexOptions.None;
        var timeout = TimeSpan.FromMilliseconds(500);
        
        var sanitisedServiceId = Regex.Replace(serviceId, regex, "", regexOptions, timeout);
        var sanitisedCaseNumber = Regex.Replace(caseNumber, regex, "", regexOptions, timeout);
        
        return $"{configuration.StreamingEndpoint}{configuration.ApplicationName}/{sanitisedServiceId}-{sanitisedCaseNumber}-{hearingId}";
    }
}
