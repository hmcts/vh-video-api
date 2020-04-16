using System;
using System.Threading.Tasks;
using VideoApi.Contract.Responses;
using VideoApi.Services.Contracts;
using VideoApi.Services.Responses;

namespace VideoApi.Services
{
    public class WowzaStreamingServiceStub : IAudioStreamService
    {
        public async Task<WowzaGetApplicationResponse> GetApplicationAsync(string applicationName)
        {
            return await Task.FromResult(new WowzaGetApplicationResponse
            {
                Name = "MyApplicationName"
            });
        }

        public async Task<WowzaGetApplicationsResponse> GetApplicationsAsync()
        {
            return await Task.FromResult(new WowzaGetApplicationsResponse
            {
                ServerName = "Server"
            });
        }

        public async Task<AudioStreamServiceResponse> CreateConferenceStreamAsync(string caseNumber, Guid hearingId)
        {
            return await Task.FromResult(new AudioStreamServiceResponse(true));
        }

        public async Task<WowzaMonitorStreamResponse> MonitoringStreamRecorderAsync(string applicationName)
        {
            return await Task.FromResult(new WowzaMonitorStreamResponse
            {
                Name = "MyApplicationStreamName"
            });
        }

        public async Task<AudioStreamServiceResponse> StopStreamRecorderAsync(string applicationName)
        {
            return await Task.FromResult(new AudioStreamServiceResponse(true));
        }
    }
}
