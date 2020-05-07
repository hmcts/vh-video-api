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
        
        public async Task<WowzaGetApplicationResponse> GetAudioApplicationInfoAsync(Guid hearingId)
        {
            try
            {
                var response = await _wowzaClient.GetApplicationAsync(hearingId.ToString(), _wowzaConfiguration.ServerName, _wowzaConfiguration.HostName);

                _logger.LogInformation($"Got Wowza application info: {hearingId}");

                return response;
            }
            catch (AudioPlatformException ex)
            {
                var errorMessage = $"Failed to get info for Wowza application: {hearingId}, " +
                                   $"StatusCode: {ex.StatusCode}, Error: {ex.Message}";
                
                _logger.LogError(ex, errorMessage);

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
                
                _logger.LogError(ex, errorMessage);

                return null;
            }
        }

        public async Task<AudioPlatformServiceResponse> CreateAudioApplicationAsync(Guid hearingId)
        {
            try
            {
               await CreateAndUpdateApplicationAsync(hearingId.ToString());
                
               return new AudioPlatformServiceResponse(true);
            }
            catch (AudioPlatformException ex)
            {
                var errorMessage = "Failed to create the Wowza application for: " +
                                   $"{hearingId}, StatusCode: {ex.StatusCode}, " +
                                   $"Error: {ex.Message}";
                
                _logger.LogError(ex, errorMessage);

                return new AudioPlatformServiceResponse(false)
                {
                    Message = errorMessage, StatusCode = ex.StatusCode
                };
            }
        }

        public async Task<AudioPlatformServiceResponse> CreateAudioApplicationWithStreamAsync(Guid hearingId)
        {
            try
            {
                await CreateAndUpdateApplicationAsync(hearingId.ToString());

                await _wowzaClient.AddStreamRecorderAsync(hearingId.ToString(), _wowzaConfiguration.ServerName, _wowzaConfiguration.HostName);
                _logger.LogInformation($"Created a Wowza stream recorder for: {hearingId}");

                return new AudioPlatformServiceResponse(true) { IngestUrl = GetAudioIngestUrl(hearingId.ToString()) };
            }
            catch (AudioPlatformException ex)
            {
                var errorMessage = "Failed to create the Wowza application and/or stream recorder for: " +
                                   $"{hearingId}, StatusCode: {ex.StatusCode}, " +
                                   $"Error: {ex.Message}";
                
                _logger.LogError(ex, errorMessage);

                return new AudioPlatformServiceResponse(false)
                {
                    Message = errorMessage, StatusCode = ex.StatusCode, IngestUrl = DefaultEmptyIngestUrl
                };
            }
        }

        public async Task<AudioPlatformServiceResponse> DeleteAudioApplicationAsync(Guid hearingId)
        {
            try
            {
                await _wowzaClient.DeleteApplicationAsync(hearingId.ToString(), _wowzaConfiguration.ServerName, _wowzaConfiguration.HostName);

                _logger.LogInformation($"Deleted Wowza application: {hearingId}");

                return new AudioPlatformServiceResponse(true);
            }
            catch (AudioPlatformException ex)
            {
                var errorMessage = $"Failed to delete the Wowza application: {hearingId}, " +
                                   $"StatusCode: {ex.StatusCode}, Error: {ex.Message}";
                
                _logger.LogError(ex, errorMessage);

                return new AudioPlatformServiceResponse(false){ Message = errorMessage, StatusCode = ex.StatusCode };
            }
        }

        public async Task<WowzaMonitorStreamResponse> GetAudioStreamMonitoringInfoAsync(Guid hearingId)
        {
            try
            {
                var response = await _wowzaClient.MonitoringStreamRecorderAsync(hearingId.ToString(), _wowzaConfiguration.ServerName, _wowzaConfiguration.HostName);

                _logger.LogInformation($"Got Wowza monitor stream data for application: {hearingId}");

                return response;
            }
            catch (AudioPlatformException ex)
            {
                var errorMessage = $"Failed to get Wowza monitor stream data for application {hearingId}, " +
                                   $"StatusCode: {ex.StatusCode}, Error: {ex.Message}";
                
                _logger.LogError(ex, errorMessage);

                return null;
            }
        }

        public async Task<WowzaGetStreamRecorderResponse> GetAudioStreamInfoAsync(Guid hearingId)
        {
            try
            {
                var response = await _wowzaClient.GetStreamRecorderAsync(hearingId.ToString(), _wowzaConfiguration.ServerName, _wowzaConfiguration.HostName);

                _logger.LogInformation($"Got Wowza stream recorder for application: {hearingId}");

                return response;
            }
            catch (AudioPlatformException ex)
            {
                var errorMessage = $"Failed to get the Wowza stream recorder for application: {hearingId}, " +
                                   $"StatusCode: {ex.StatusCode}, Error: {ex.Message}";
                
                _logger.LogError(ex, errorMessage);

                return null;
            }
        }

        public async Task<AudioPlatformServiceResponse> CreateAudioStreamAsync(Guid hearingId)
        {
            try
            {
                await _wowzaClient.AddStreamRecorderAsync(hearingId.ToString(), _wowzaConfiguration.ServerName, _wowzaConfiguration.HostName);
                _logger.LogInformation($"Created a Wowza stream recorder for: {hearingId}");

                return new AudioPlatformServiceResponse(true) { IngestUrl = GetAudioIngestUrl(hearingId.ToString()) };
            }
            catch (AudioPlatformException ex)
            {
                var errorMessage = "Failed to create the Wowza stream recorder for: " +
                                   $"{hearingId}, StatusCode: {ex.StatusCode}, " +
                                   $"Error: {ex.Message}";
                
                _logger.LogError(ex, errorMessage);

                return new AudioPlatformServiceResponse(false)
                {
                    Message = errorMessage, StatusCode = ex.StatusCode, IngestUrl = DefaultEmptyIngestUrl
                };
            }
        }

        public async Task<AudioPlatformServiceResponse> DeleteAudioStreamAsync(Guid hearingId)
        {
            try
            {
                await _wowzaClient.StopStreamRecorderAsync(hearingId.ToString(), _wowzaConfiguration.ServerName, _wowzaConfiguration.HostName);

                _logger.LogInformation($"Stopped Wowza stream recorder for application: {hearingId}");

                return new AudioPlatformServiceResponse(true);
            }
            catch (AudioPlatformException ex)
            {
                var errorMessage = $"Failed to get the Wowza stream recorder for application: {hearingId}, " +
                                   $"StatusCode: {ex.StatusCode}, Error: {ex.Message}";
                
                _logger.LogError(ex, errorMessage);

                return new AudioPlatformServiceResponse(false){ Message = errorMessage, StatusCode = ex.StatusCode };
            }
        }

        private async Task CreateAndUpdateApplicationAsync(string applicationName)
        {
            await _wowzaClient.CreateApplicationAsync(applicationName, _wowzaConfiguration.ServerName, _wowzaConfiguration.HostName, _wowzaConfiguration.StorageDirectory);
            _logger.LogInformation($"Created a Wowza application for: {applicationName}");

            await _wowzaClient.UpdateApplicationAsync(applicationName, _wowzaConfiguration.ServerName, _wowzaConfiguration.HostName, _wowzaConfiguration.AzureStorageDirectory);
            _logger.LogInformation($"Updating Wowza application for: {applicationName}");
        }

        private string GetAudioIngestUrl(string applicationName) => $"{_wowzaConfiguration.StreamingEndpoint}{applicationName}/{applicationName}";
    }
}
