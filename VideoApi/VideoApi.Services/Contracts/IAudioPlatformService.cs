using System;
using System.Threading.Tasks;
using VideoApi.Contract.Responses;
using VideoApi.Services.Responses;

namespace VideoApi.Services.Contracts
{
    public interface IAudioPlatformService
    {  
        Task<AudioPlatformServiceResponse> DeleteAudioApplicationAsync(Guid hearingId);        
        Task<WowzaMonitorStreamResponse> GetAudioStreamMonitoringInfoAsync(Guid hearingId);        
        Task<WowzaGetStreamRecorderResponse> GetAudioStreamInfoAsync(Guid hearingId);
        public string GetAudioIngestUrl(string hearingId);
        public string ApplicationName { get; }
        Task<AudioPlatformServiceResponse> DeleteAudioStreamAsync(Guid hearingId);
        Task<bool> GetDiagnosticsAsync();
    }
}
