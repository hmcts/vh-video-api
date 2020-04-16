using System;
using System.Threading.Tasks;
using VideoApi.Contract.Responses;
using VideoApi.Services.Responses;

namespace VideoApi.Services.Contracts
{
    public interface IAudioStreamService
    {
        Task<WowzaGetApplicationResponse> GetApplicationAsync(string applicationName);
        Task<WowzaGetApplicationsResponse> GetApplicationsAsync();
        Task<AudioStreamServiceResponse> CreateConferenceStreamAsync(string caseNumber, Guid hearingId);
        Task<WowzaMonitorStreamResponse> MonitoringStreamRecorderAsync(string applicationName);
        Task<AudioStreamServiceResponse> StopStreamRecorderAsync(string applicationName);
    }
}
