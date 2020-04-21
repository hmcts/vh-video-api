using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VideoApi.Contract.Responses;
using VideoApi.Services.Contracts;
using VideoApi.Services.Responses;

namespace VideoApi.Services
{
    public class AudioPlatformServiceStub : IAudioPlatformService
    {
        public async Task<WowzaGetApplicationResponse> GetAudioApplicationInfoAsync(Guid hearingId)
        {
            return await Task.FromResult(new WowzaGetApplicationResponse
            {
                Name = "MyApplicationName"
            });
        }

        public async Task<WowzaGetApplicationsResponse> GetAllAudioApplicationsInfoAsync()
        {
            return await Task.FromResult(new WowzaGetApplicationsResponse
            {
                ServerName = "Server", Applications = new List<Application>
                {
                    new Application{Id = "one"}, new Application{Id = "two"}, new Application{Id = "three"}
                }.ToArray()
            });
        }

        public async Task<AudioPlatformServiceResponse> CreateAudioApplicationAsync(Guid hearingId)
        {
            return await Task.FromResult(new AudioPlatformServiceResponse(true));
        }

        public async Task<AudioPlatformServiceResponse> CreateAudioStreamAsync(Guid hearingId)
        {
            return await Task.FromResult(new AudioPlatformServiceResponse(true)
            {
                IngestUrl = $"https://localhost.streaming.mediaServices.windows.net/{Guid.NewGuid()}"
            });
        }

        public async Task<AudioPlatformServiceResponse> CreateAudioApplicationWithStreamAsync(Guid hearingId)
        {
            var applicationName = Guid.NewGuid();
            return await Task.FromResult(new AudioPlatformServiceResponse(true)
            {
                IngestUrl = $"https://localhost.streaming.mediaServices.windows.net/{applicationName}/{applicationName}"
            });
        }

        public async Task<AudioPlatformServiceResponse> DeleteAudioApplicationAsync(Guid hearingId)
        {
            return await Task.FromResult(new AudioPlatformServiceResponse(true));
        }

        public async Task<WowzaMonitorStreamResponse> GetAudioStreamMonitoringInfoAsync(Guid hearingId)
        {
            return await Task.FromResult(new WowzaMonitorStreamResponse
            {
                Name = "MyApplicationStreamName"
            });
        }

        public async Task<WowzaGetStreamRecorderResponse> GetAudioStreamInfoAsync(Guid hearingId)
        {
            return await Task.FromResult(new WowzaGetStreamRecorderResponse
            {
                ApplicationName = "MyApplicationName",
                RecordingStartTime = DateTime.UtcNow.AddMinutes(-1).ToString("s")
            });
        }

        public async Task<AudioPlatformServiceResponse> DeleteAudioStreamAsync(Guid hearingId)
        {
            return await Task.FromResult(new AudioPlatformServiceResponse(true));
        }
    }
}
