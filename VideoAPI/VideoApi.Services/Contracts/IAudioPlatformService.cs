using System;
using System.Threading.Tasks;
using VideoApi.Contract.Responses;
using VideoApi.Services.Responses;

namespace VideoApi.Services.Contracts
{
    public interface IAudioPlatformService
    {
        Task<WowzaGetApplicationResponse> GetApplicationAsync(string applicationName);
        Task<WowzaGetApplicationsResponse> GetApplicationsAsync();
        Task<string> CreateConferenceStreamAsync(string caseNumber, Guid hearingId);
        Task<WowzaMonitorStreamResponse> MonitoringStreamRecorderAsync(string applicationName);
        Task<AudioPlatformServiceResponse> StopStreamRecorderAsync(string applicationName);
    }
}
