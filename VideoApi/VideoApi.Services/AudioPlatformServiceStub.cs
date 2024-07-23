using System;
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

        public async Task<WowzaGetApplicationResponse> GetAudioApplicationInfoAsync(Guid? hearingId = null)
        {
            return await Task.FromResult(new WowzaGetApplicationResponse
            {
                Name = "MyApplicationName"
            });
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

        public async Task<WowzaGetStreamRecorderResponse> GetAudioStreamInfoAsync(string recorder)
        {
            var hearingID = new Guid(recorder);
            if (!hearingID.Equals(_audioRecordingTestIdConfiguration.Existing))
            {
                return await Task.FromResult<WowzaGetStreamRecorderResponse>(null);
            }

            return await Task.FromResult(new WowzaGetStreamRecorderResponse
            {
                ApplicationName = "MyApplicationName",
                RecordingStartTime = DateTime.UtcNow.AddMinutes(-1).ToString("s")
            });
        }
        public async Task<bool> GetDiagnosticsAsync()
        {
            return await Task.FromResult(true);
        }
        
        public string GetAudioIngestUrl(string serviceId, string caseNumber, string hearingId)
        {
            return $"https://localhost.streaming.mediaServices.windows.net/{serviceId}-{caseNumber}-{hearingId}";
        }

        public string ApplicationName { get; }
    }
}
