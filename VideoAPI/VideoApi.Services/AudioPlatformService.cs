using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VideoApi.Common.Configuration;
using VideoApi.Contract.Responses;
using VideoApi.Services.Contracts;
using VideoApi.Services.Exceptions;
using VideoApi.Services.Responses;

namespace VideoApi.Services
{
    public class AudioPlatformService : IAudioPlatformService
    {
        private readonly IWowzaHttpClient _wowzaClient;
        private readonly WowzaConfiguration _wowzaConfiguration;
        private readonly ILogger<AudioPlatformService> _logger;
        private const string DefaultEmptyIngestUrl = " ";

        public AudioPlatformService(IWowzaHttpClient wowzaClient, WowzaConfiguration wowzaConfiguration, ILogger<AudioPlatformService> logger)
        {
            _wowzaClient = wowzaClient;
            _wowzaConfiguration = wowzaConfiguration;
            _logger = logger;
        }
        
        public async Task<WowzaGetApplicationResponse> GetAudioApplicationInfoAsync(string caseNumber, Guid hearingId)
        {
            var applicationName = $"{caseNumber}_{hearingId}";
            
            try
            {
                var response = await _wowzaClient.GetApplicationAsync(applicationName, _wowzaConfiguration.ServerName, _wowzaConfiguration.HostName);

                _logger.LogInformation($"Got Wowza application info: {applicationName}");

                return response;
            }
            catch (AudioPlatformException ex)
            {
                var errorMessage = $"Failed to get info for Wowza application: {applicationName}, " +
                                   $"StatusCode: {ex.StatusCode}, Error: {ex.Message}";
                
                _logger.LogError(errorMessage, ex);

                return null;
            }
        }

        public async Task<WowzaGetApplicationsResponse> GetAllAudioApplicationsInfoAsync()
        {
            try
            {
                var response = await _wowzaClient.GetApplicationsAsync(_wowzaConfiguration.ServerName, _wowzaConfiguration.HostName);

                _logger.LogInformation("Got all Wowza applications info");

                return response;
            }
            catch (AudioPlatformException ex)
            {
                var errorMessage = $"Failed to get all Wowza applications info, StatusCode: {ex.StatusCode}, " +
                                   $"Error: {ex.Message}";
                
                _logger.LogError(errorMessage, ex);

                return null;
            }
        }

        public async Task<AudioPlatformServiceResponse> CreateAudioApplicationAsync(string caseNumber, Guid hearingId)
        {
            var applicationName = $"{caseNumber}_{hearingId}";

            try
            {
                await _wowzaClient.CreateApplicationAsync(applicationName, _wowzaConfiguration.ServerName, _wowzaConfiguration.HostName, _wowzaConfiguration.StorageDirectory);
                _logger.LogInformation($"Created a Wowza application for: {applicationName}");
                
                return new AudioPlatformServiceResponse(true);
            }
            catch (AudioPlatformException ex)
            {
                var errorMessage = "Failed to create the Wowza application for: " +
                                   $"{applicationName}, StatusCode: {ex.StatusCode}, " +
                                   $"Error: {ex.Message}";
                
                _logger.LogError(errorMessage, ex);

                return new AudioPlatformServiceResponse(false)
                {
                    Message = errorMessage, StatusCode = ex.StatusCode
                };
            }
        }

        public async Task<AudioPlatformServiceResponse> CreateAudioApplicationWithStreamAsync(string caseNumber, Guid hearingId)
        {
            var applicationName = $"{caseNumber}_{hearingId}";

            try
            {
                await _wowzaClient.CreateApplicationAsync(applicationName, _wowzaConfiguration.ServerName, _wowzaConfiguration.HostName, _wowzaConfiguration.StorageDirectory);
                _logger.LogInformation($"Created a Wowza application for: {applicationName}");
                
                await _wowzaClient.AddStreamRecorderAsync(applicationName, _wowzaConfiguration.ServerName, _wowzaConfiguration.HostName);
                _logger.LogInformation($"Created a Wowza stream recorder for: {applicationName}");

                return new AudioPlatformServiceResponse(true) { IngestUrl = GetAudioIngestUrl(applicationName) };
            }
            catch (AudioPlatformException ex)
            {
                var errorMessage = "Failed to create the Wowza application and/or stream recorder for: " +
                                   $"{applicationName}, StatusCode: {ex.StatusCode}, " +
                                   $"Error: {ex.Message}";
                
                _logger.LogError(errorMessage, ex);

                return new AudioPlatformServiceResponse(false)
                {
                    Message = errorMessage, StatusCode = ex.StatusCode, IngestUrl = DefaultEmptyIngestUrl
                };
            }
        }

        public async Task<AudioPlatformServiceResponse> DeleteAudioApplicationAsync(string caseNumber, Guid hearingId)
        {
            var applicationName = $"{caseNumber}_{hearingId}";
            
            try
            {
                await _wowzaClient.DeleteApplicationAsync(applicationName, _wowzaConfiguration.ServerName, _wowzaConfiguration.HostName);

                _logger.LogInformation($"Deleted Wowza application: {applicationName}");

                return new AudioPlatformServiceResponse(true);
            }
            catch (AudioPlatformException ex)
            {
                var errorMessage = $"Failed to delete the Wowza application: {applicationName}, " +
                                   $"StatusCode: {ex.StatusCode}, Error: {ex.Message}";
                
                _logger.LogError(errorMessage, ex);

                return new AudioPlatformServiceResponse(false){ Message = errorMessage, StatusCode = ex.StatusCode };
            }
        }

        public async Task<WowzaMonitorStreamResponse> GetAudioStreamMonitoringInfoAsync(string caseNumber, Guid hearingId)
        {
            var applicationName = $"{caseNumber}_{hearingId}";
            
            try
            {
                var response = await _wowzaClient.MonitoringStreamRecorderAsync(applicationName, _wowzaConfiguration.ServerName, _wowzaConfiguration.HostName);

                _logger.LogInformation($"Got Wowza monitor stream data for application: {applicationName}");

                return response;
            }
            catch (AudioPlatformException ex)
            {
                var errorMessage = $"Failed to get Wowza monitor stream data for application {applicationName}, " +
                                   $"StatusCode: {ex.StatusCode}, Error: {ex.Message}";
                
                _logger.LogError(errorMessage, ex);

                return null;
            }
        }

        public async Task<WowzaGetStreamRecorderResponse> GetAudioStreamInfoAsync(string caseNumber, Guid hearingId)
        {
            var applicationName = $"{caseNumber}_{hearingId}";
            
            try
            {
                var response = await _wowzaClient.GetStreamRecorderAsync(applicationName, _wowzaConfiguration.ServerName, _wowzaConfiguration.HostName);

                _logger.LogInformation($"Got Wowza stream recorder for application: {applicationName}");

                return response;
            }
            catch (AudioPlatformException ex)
            {
                var errorMessage = $"Failed to get the Wowza stream recorder for application: {applicationName}, " +
                                   $"StatusCode: {ex.StatusCode}, Error: {ex.Message}";
                
                _logger.LogError(errorMessage, ex);

                return null;
            }
        }

        public async Task<AudioPlatformServiceResponse> CreateAudioStreamAsync(string caseNumber, Guid hearingId)
        {
            var applicationName = $"{caseNumber}_{hearingId}";

            try
            {
                await _wowzaClient.AddStreamRecorderAsync(applicationName, _wowzaConfiguration.ServerName, _wowzaConfiguration.HostName);
                _logger.LogInformation($"Created a Wowza stream recorder for: {applicationName}");

                return new AudioPlatformServiceResponse(true) { IngestUrl = GetAudioIngestUrl(applicationName) };
            }
            catch (AudioPlatformException ex)
            {
                var errorMessage = "Failed to create the Wowza application and/or stream recorder for: " +
                                   $"{applicationName}, StatusCode: {ex.StatusCode}, " +
                                   $"Error: {ex.Message}";
                
                _logger.LogError(errorMessage, ex);

                return new AudioPlatformServiceResponse(false)
                {
                    Message = errorMessage, StatusCode = ex.StatusCode, IngestUrl = DefaultEmptyIngestUrl
                };
            }
        }

        public async Task<AudioPlatformServiceResponse> DeleteAudioStreamAsync(string caseNumber, Guid hearingId)
        {
            var applicationName = $"{caseNumber}_{hearingId}";
            
            try
            {
                await _wowzaClient.StopStreamRecorderAsync(applicationName, _wowzaConfiguration.ServerName, _wowzaConfiguration.HostName);

                _logger.LogInformation($"Stopped Wowza stream recorder for application: {applicationName}");

                return new AudioPlatformServiceResponse(true);
            }
            catch (AudioPlatformException ex)
            {
                var errorMessage = $"Failed to get the Wowza stream recorder for application: {applicationName}, " +
                                   $"StatusCode: {ex.StatusCode}, Error: {ex.Message}";
                
                _logger.LogError(errorMessage, ex);

                return new AudioPlatformServiceResponse(false){ Message = errorMessage, StatusCode = ex.StatusCode };
            }
        }

        private string GetAudioIngestUrl(string applicationName) => $"{_wowzaConfiguration.StreamingEndpoint}{applicationName}";
    }
}
