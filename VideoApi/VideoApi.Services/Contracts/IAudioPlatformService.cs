using System;
using System.Threading.Tasks;
using VideoApi.Contract.Responses;
using VideoApi.Services.Responses;

namespace VideoApi.Services.Contracts
{
    public interface IAudioPlatformService
    {
        Task<WowzaGetApplicationResponse> GetAudioApplicationInfoAsync(Guid? hearingId = null);   
        Task<AudioPlatformServiceResponse> DeleteAudioApplicationAsync(Guid hearingId);        
        Task<WowzaMonitorStreamResponse> GetAudioStreamMonitoringInfoAsync(Guid hearingId);        
        Task<WowzaGetStreamRecorderResponse> GetAudioStreamInfoAsync(Guid hearingId);
        public string GetAudioIngestUrl(string hearingId);
        public string ApplicationName { get; }
        Task<bool> GetDiagnosticsAsync();
    }
}
