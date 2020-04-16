using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VideoApi.Contract.Responses;
using VideoApi.Services.Contracts;
using VideoApi.Services.Exceptions;

namespace VideoApi.Services
{
    public class WowzaStreamingService : IAudioStreamService
    {
        private readonly IWowzaHttpClient _wowzaClient;
        private readonly ILogger<WowzaStreamingService> _logger;

        public WowzaStreamingService(IWowzaHttpClient wowzaClient, ILogger<WowzaStreamingService> logger)
        {
            _wowzaClient = wowzaClient;
            _logger = logger;
        }

        public async Task<AudioStreamServiceResponse> GetApplicationAsync(string applicationName)
        {
            try
            {
                var response = await _wowzaClient.GetApplicationAsync(applicationName);

                _logger.LogInformation($"Get Wowza application info: {applicationName}");

                return new AudioStreamServiceResponse(true, data: response);
            }
            catch (StreamingEngineException ex)
            {
                var errorMessage = $"Failed to get info for Wowza application: {applicationName}, " +
                                   $"StatusCode: {ex.StatusCode}, Error: {ex.Message}";
                
                _logger.LogError(errorMessage, ex);

                return new AudioStreamServiceResponse(false, errorMessage);
            }
        }

        public async Task<AudioStreamServiceResponse> GetApplicationsAsync()
        {
            try
            {
                var response = await _wowzaClient.GetApplicationsAsync();

                _logger.LogInformation("Get all Wowza applications info");

                return new AudioStreamServiceResponse(true, data: response);
            }
            catch (StreamingEngineException ex)
            {
                var errorMessage = $"Failed to get all Wowza applications info, StatusCode: {ex.StatusCode}, " +
                                   $"Error: {ex.Message}";
                
                _logger.LogError(errorMessage, ex);

                return new AudioStreamServiceResponse(false, errorMessage);
            }
        }

        public async Task<AudioStreamServiceResponse> CreateConferenceStreamAsync(string caseNumber, Guid hearingId)
        {
            var applicationName = $"{caseNumber}_{hearingId}";

            try
            {
                await _wowzaClient.CreateApplicationAsync(applicationName);
                _logger.LogInformation($"Created a Wowza application for: {applicationName}");
                
                await _wowzaClient.AddStreamRecorderAsync(applicationName);
                _logger.LogInformation($"Created a Wowza stream recorder for: {applicationName}");
                
                return new AudioStreamServiceResponse(true);
            }
            catch (StreamingEngineException ex)
            {
                var errorMessage = "Failed to create the Wowza application and/or stream recorder for: " +
                                   $"{applicationName}, StatusCode: {ex.StatusCode}, " +
                                   $"Error: {ex.Message}";
                
                _logger.LogError(errorMessage, ex);

                return new AudioStreamServiceResponse(false, errorMessage);
            }
        }

        public async Task<AudioStreamServiceResponse> MonitoringStreamRecorderAsync(string applicationName)
        {
            try
            {
                var response = await _wowzaClient.MonitoringStreamRecorderAsync(applicationName);

                _logger.LogInformation($"Get Wowza monitor stream data for application: {applicationName}");

                return new AudioStreamServiceResponse(true, data: response);
            }
            catch (StreamingEngineException ex)
            {
                var errorMessage = $"Failed to get Wowza monitor stream data for application {applicationName}, " +
                                   $"StatusCode: {ex.StatusCode}, Error: {ex.Message}";
                
                _logger.LogError(errorMessage, ex);

                return new AudioStreamServiceResponse(false, errorMessage);
            }
        }

        public async Task<AudioStreamServiceResponse> StopStreamRecorderAsync(string applicationName)
        {
            try
            {
                await _wowzaClient.StopStreamRecorderAsync(applicationName);

                _logger.LogInformation($"Stopped Wowza stream recorder for application: {applicationName}");

                return new AudioStreamServiceResponse(true);
            }
            catch (StreamingEngineException ex)
            {
                var errorMessage = $"Failed to the Wowza stream recorder for application: {applicationName}, " +
                                   $"StatusCode: {ex.StatusCode}, Error: {ex.Message}";
                
                _logger.LogError(errorMessage, ex);

                return new AudioStreamServiceResponse(false, errorMessage);
            }
        }
    }
}
