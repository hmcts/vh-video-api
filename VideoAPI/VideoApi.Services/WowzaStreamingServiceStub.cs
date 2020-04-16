using System;
using System.Threading.Tasks;
using VideoApi.Contract.Responses;
using VideoApi.Services.Contracts;

namespace VideoApi.Services
{
    public class WowzaStreamingServiceStub : IAudioStreamService
    {
        public async Task<AudioStreamServiceResponse> GetApplicationAsync(string applicationName)
        {
            return await Task.FromResult(new AudioStreamServiceResponse(true));
        }

        public async Task<AudioStreamServiceResponse> GetApplicationsAsync()
        {
            return await Task.FromResult(new AudioStreamServiceResponse(true));
        }

        public async Task<AudioStreamServiceResponse> CreateConferenceStreamAsync(string caseNumber, Guid hearingId)
        {
            return await Task.FromResult(new AudioStreamServiceResponse(true));
        }

        public async Task<AudioStreamServiceResponse> MonitoringStreamRecorderAsync(string applicationName)
        {
            return await Task.FromResult(new AudioStreamServiceResponse(true));
        }

        public async Task<AudioStreamServiceResponse> StopStreamRecorderAsync(string applicationName)
        {
            return await Task.FromResult(new AudioStreamServiceResponse(true));
        }
    }
}
