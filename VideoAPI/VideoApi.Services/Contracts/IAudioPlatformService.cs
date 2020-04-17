using System;
using System.Threading.Tasks;
using VideoApi.Contract.Responses;
using VideoApi.Services.Responses;

namespace VideoApi.Services.Contracts
{
    public interface IAudioPlatformService
    {
        Task<WowzaGetApplicationResponse> GetAudioApplicationInfoAsync(string caseNumber, Guid hearingId);
        Task<WowzaGetApplicationsResponse> GetAllAudioApplicationsInfoAsync();
        
        Task<AudioPlatformServiceResponse> CreateAudioApplicationAsync(string caseNumber, Guid hearingId);
        Task<AudioPlatformServiceResponse> CreateAudioApplicationWithStreamAsync(string caseNumber, Guid hearingId);
        Task<AudioPlatformServiceResponse> DeleteAudioApplicationAsync(string caseNumber, Guid hearingId);
        
        Task<WowzaMonitorStreamResponse> GetAudioStreamMonitoringInfoAsync(string caseNumber, Guid hearingId);
        
        Task<WowzaGetStreamRecorderResponse> GetAudioStreamInfoAsync(string caseNumber, Guid hearingId);
        Task<AudioPlatformServiceResponse> CreateAudioStreamAsync(string caseNumber, Guid hearingId);
        Task<AudioPlatformServiceResponse> DeleteAudioStreamAsync(string caseNumber, Guid hearingId);
    }
}
