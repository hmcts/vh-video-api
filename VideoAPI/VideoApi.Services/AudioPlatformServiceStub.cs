using System;
using System.Threading.Tasks;
using VideoApi.Contract.Responses;
using VideoApi.Services.Contracts;
using VideoApi.Services.Responses;

namespace VideoApi.Services
{
    public class AudioPlatformServiceStub : IAudioPlatformService
    {
        public async Task<WowzaGetApplicationResponse> GetAudioApplicationAsync(string applicationName)
        {
            return await Task.FromResult(new WowzaGetApplicationResponse
            {
                Name = "MyApplicationName"
            });
        }

        public async Task<WowzaGetApplicationsResponse> GetAllAudioApplicationsAsync()
        {
            return await Task.FromResult(new WowzaGetApplicationsResponse
            {
                ServerName = "Server"
            });
        }

        public async Task<string> CreateAudioStreamAsync(string caseNumber, Guid hearingId)
        {
            return await Task.FromResult($"https://localhost.streaming.mediaServices.windows.net/{Guid.NewGuid()}");
        }

        public async Task<WowzaMonitorStreamResponse> GetAudioStreamRealtimeInfoAsync(string applicationName)
        {
            return await Task.FromResult(new WowzaMonitorStreamResponse
            {
                Name = "MyApplicationStreamName"
            });
        }

        public async Task<WowzaGetStreamRecorderResponse> GetAudioStreamInfoAsync(string applicationName)
        {
            return await Task.FromResult(new WowzaGetStreamRecorderResponse
            {
                ApplicationName = "MyApplicationName"
            });
        }

        public async Task<AudioPlatformServiceResponse> StopAudioStreamAsync(string applicationName)
        {
            return await Task.FromResult(new AudioPlatformServiceResponse(true));
        }
    }
}
