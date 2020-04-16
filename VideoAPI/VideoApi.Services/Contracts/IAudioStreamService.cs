using System;
using System.Threading.Tasks;
using VideoApi.Contract.Responses;

namespace VideoApi.Services.Contracts
{
    public interface IAudioStreamService
    {
        Task<AudioStreamServiceResponse> GetApplicationAsync(string applicationName);
        Task<AudioStreamServiceResponse> GetApplicationsAsync();
        Task<AudioStreamServiceResponse> CreateConferenceStreamAsync(string caseNumber, Guid hearingId);
        Task<AudioStreamServiceResponse> MonitoringStreamRecorderAsync(string applicationName);
        Task<AudioStreamServiceResponse> StopStreamRecorderAsync(string applicationName);
    }
}
