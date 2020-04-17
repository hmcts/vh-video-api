using System;
using System.Threading.Tasks;
using VideoApi.Contract.Responses;
using VideoApi.Services.Responses;

namespace VideoApi.Services.Contracts
{
    public interface IAudioPlatformService
    {
        Task<WowzaGetApplicationResponse> GetAudioApplicationAsync(string applicationName);
        Task<WowzaGetApplicationsResponse> GetAllAudioApplicationsAsync();
        Task<string> CreateAudioStreamAsync(string caseNumber, Guid hearingId);
        Task<WowzaMonitorStreamResponse> GetAudioStreamRealtimeInfoAsync(string applicationName);
        Task<WowzaGetStreamRecorderResponse> GetAudioStreamInfoAsync(string applicationName);
        Task<AudioPlatformServiceResponse> StopAudioStreamAsync(string applicationName);
    }
}
