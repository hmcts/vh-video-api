using System;
using System.Threading.Tasks;
using VideoApi.Contract.Responses;
using VideoApi.Services.Responses;

namespace VideoApi.Services.Contracts
{
    public interface IAudioPlatformService
    {
        Task<WowzaGetApplicationResponse> GetAudioApplicationInfoAsync(Guid hearingId);
        Task<WowzaGetApplicationsResponse> GetAllAudioApplicationsInfoAsync();
        
        Task<AudioPlatformServiceResponse> CreateAudioApplicationAsync(Guid hearingId);
        Task<AudioPlatformServiceResponse> CreateAudioApplicationWithStreamAsync(Guid hearingId);
        Task<AudioPlatformServiceResponse> DeleteAudioApplicationAsync(Guid hearingId);
        
        Task<WowzaMonitorStreamResponse> GetAudioStreamMonitoringInfoAsync(Guid hearingId);
        
        Task<WowzaGetStreamRecorderResponse> GetAudioStreamInfoAsync(Guid hearingId);
        Task<AudioPlatformServiceResponse> CreateAudioStreamAsync(Guid hearingId);
        Task<AudioPlatformServiceResponse> DeleteAudioStreamAsync(Guid hearingId);
        Task<WowzaGetDiagnosticsResponse> GetDiagnosticsAsync();
    }
}
