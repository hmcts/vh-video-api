using System;
using System.Threading.Tasks;
using VideoApi.Contract.Responses;
using VideoApi.Services.Responses;

namespace VideoApi.Services.Contracts
{
    public interface IAudioPlatformService
    {
        Task<WowzaGetApplicationResponse> GetAudioApplicationAsync(string caseNumber, Guid hearingId);
        Task<WowzaGetApplicationsResponse> GetAllAudioApplicationsAsync();
        Task<AudioPlatformServiceResponse> CreateAudioStreamAsync(string caseNumber, Guid hearingId);
        Task<AudioPlatformServiceResponse> DeleteAudioStreamAsync(string caseNumber, Guid hearingId);
        Task<WowzaMonitorStreamResponse> GetAudioStreamRealtimeInfoAsync(string caseNumber, Guid hearingId);
        Task<WowzaGetStreamRecorderResponse> GetAudioStreamInfoAsync(string caseNumber, Guid hearingId);
        Task<AudioPlatformServiceResponse> StopAudioStreamAsync(string caseNumber, Guid hearingId);
    }
}
