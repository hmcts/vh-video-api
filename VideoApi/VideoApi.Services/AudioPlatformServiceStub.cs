using System.Diagnostics.CodeAnalysis;
using VideoApi.Services.Contracts;

namespace VideoApi.Services
{
    [ExcludeFromCodeCoverage]
    public class AudioPlatformServiceStub : IAudioPlatformService
    {
        public string ApplicationName { get; } = "vh-recording-app";
        
        public string GetAudioIngestUrl(string serviceId, string caseNumber, string hearingId)
        {
            return $"https://localhost.streaming.mediaServices.windows.net/{serviceId}-{caseNumber}-{hearingId}";
        }
    }
}
