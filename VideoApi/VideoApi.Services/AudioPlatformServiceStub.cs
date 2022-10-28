using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using VideoApi.Common.Configuration;
using VideoApi.Contract.Responses;
using VideoApi.Services.Contracts;
using VideoApi.Services.Responses;

namespace VideoApi.Services
{
    [ExcludeFromCodeCoverage]
    public class AudioPlatformServiceStub : IAudioPlatformService
    {
        private readonly AudioRecordingTestIdConfiguration _audioRecordingTestIdConfiguration;
        
        public AudioPlatformServiceStub()
        {
            _audioRecordingTestIdConfiguration = new AudioRecordingTestIdConfiguration();
            ApplicationName = "vh-recording-app";
        }

        public async Task<WowzaGetApplicationResponse> GetAudioApplicationInfoAsync()
        {
            return await Task.FromResult(new WowzaGetApplicationResponse
            {
                Name = "MyApplicationName"
            });
        }

        public async Task<AudioPlatformServiceResponse> CreateAudioApplicationAsync(Guid hearingId)
        {
            if (hearingId.Equals(_audioRecordingTestIdConfiguration.Existing))
            {
                return await Task.FromResult(new AudioPlatformServiceResponse(false)
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Message = "Conflict"
                });
            }
            return await Task.FromResult(new AudioPlatformServiceResponse(true));
        }

        public async Task<AudioPlatformServiceResponse> DeleteAudioApplicationAsync(Guid hearingId)
        {
            if (hearingId.Equals(_audioRecordingTestIdConfiguration.NonExistent))
            {
                return await Task.FromResult(new AudioPlatformServiceResponse(false){ 
                    StatusCode = HttpStatusCode.NotFound
                });
            }
            return await Task.FromResult(new AudioPlatformServiceResponse(true));
        }

        public async Task<WowzaMonitorStreamResponse> GetAudioStreamMonitoringInfoAsync(Guid hearingId)
        {
            if (hearingId.Equals(_audioRecordingTestIdConfiguration.NonExistent))
            {
                return await Task.FromResult<WowzaMonitorStreamResponse>(null);
            }

            return await Task.FromResult(new WowzaMonitorStreamResponse
            {
                Name = "MyApplicationStreamName"
            });
        }

        
        public async Task<WowzaGetStreamRecorderResponse> GetAudioStreamInfoAsync(Guid hearingId)
        {
            if (hearingId.Equals(_audioRecordingTestIdConfiguration.NonExistent))
            {
                return await Task.FromResult<WowzaGetStreamRecorderResponse>(null);
            }

            return await Task.FromResult(new WowzaGetStreamRecorderResponse
            {
                ApplicationName = "MyApplicationName",
                RecordingStartTime = DateTime.UtcNow.AddMinutes(-1).ToString("s")
            });
        }

        public async Task<AudioPlatformServiceResponse> DeleteAudioStreamAsync(Guid hearingId)
        {
            return await DeleteAudioApplicationAsync(hearingId);
        }

        public async Task<IEnumerable<WowzaGetDiagnosticsResponse>> GetDiagnosticsAsync()
        {
            return await Task.FromResult(new []
            {
                new WowzaGetDiagnosticsResponse
                {
                    ServerVersion = "1.0.0.1"
                },
                new WowzaGetDiagnosticsResponse
                {
                    ServerVersion = "1.0.0.2"
                }
            });
        }

        public string GetAudioIngestUrl(string hearingId)
        {
            return $"https://localhost.streaming.mediaServices.windows.net/{hearingId}";
        }

        public string ApplicationName { get; }
    }
}
