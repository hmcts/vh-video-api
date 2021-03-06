using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private readonly IEnumerable<IWowzaHttpClient> _wowzaClients;
        private readonly WowzaConfiguration _configuration;
        private readonly ILogger<AudioPlatformService> _logger;

        public AudioPlatformService(IEnumerable<IWowzaHttpClient> wowzaClients, WowzaConfiguration configuration, ILogger<AudioPlatformService> logger)
        {
            _wowzaClients = wowzaClients;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<WowzaGetApplicationResponse> GetAudioApplicationInfoAsync(Guid hearingId)
        {
            try
            {
                var tasks = _wowzaClients
                    .Select(x => x.GetApplicationAsync(hearingId.ToString(), _configuration.ServerName, _configuration.HostName))
                    .ToList();

                var response = await WaitAnyFirstValidResult(tasks);

                _logger.LogInformation("Got Wowza application info: {hearingId}", hearingId);

                return response;
            }
            catch (AudioPlatformException ex)
            {
                var errorMessageTemplate = "Failed to get info for Wowza application: {hearingId}, StatusCode: {ex.StatusCode}, Error: {ex.Message}";

                LogError(ex, errorMessageTemplate, hearingId, ex.StatusCode, ex.Message);

                return null;
            }
        }

        public async Task<AudioPlatformServiceResponse> CreateAudioApplicationAsync(Guid hearingId)
        {
            try
            {
               await CreateAndUpdateApplicationAsync(hearingId.ToString());

               return new AudioPlatformServiceResponse(true) {IngestUrl = GetAudioIngestUrl(hearingId.ToString())};
            }
            catch (AudioPlatformException ex)
            {
                if (ex.StatusCode == HttpStatusCode.Conflict)
                {
                    return new AudioPlatformServiceResponse(true) {IngestUrl = GetAudioIngestUrl(hearingId.ToString())};
                }
                var errorMessageTemplate = "Failed to create the Wowza application for: {hearingId}, StatusCode: {ex.StatusCode}, Error: {ex.Message}";
                var errorMessage = $"Failed to create the Wowza application for: {hearingId}, StatusCode: {ex.StatusCode}, Error: {ex.Message}";
                LogError(ex, errorMessageTemplate, hearingId, ex.StatusCode, ex.Message);
                return new AudioPlatformServiceResponse(false) { Message = errorMessage, StatusCode = ex.StatusCode };
            }
        }

        public async Task<AudioPlatformServiceResponse> DeleteAudioApplicationAsync(Guid hearingId)
        {
            try
            {
                var tasks = _wowzaClients.Select(x => x.DeleteApplicationAsync(hearingId.ToString(), _configuration.ServerName, _configuration.HostName));
                await Task.WhenAll(tasks);
                
                _logger.LogInformation("Deleted Wowza application: {hearingId}", hearingId);

                return new AudioPlatformServiceResponse(true);
            }
            catch (AudioPlatformException ex)
            {
                var errorMessageTemplate = "Failed to delete the Wowza application: {hearingId}, StatusCode: {ex.StatusCode}, Error: {ex.Message}";
                var errorMessage = $"Failed to delete the Wowza application: {hearingId}, StatusCode: {ex.StatusCode}, Error: {ex.Message}";
                LogError(ex, errorMessageTemplate, hearingId, ex.StatusCode, ex.Message);
                return new AudioPlatformServiceResponse(false){ Message = errorMessage, StatusCode = ex.StatusCode };
            }
        }

        public async Task<WowzaMonitorStreamResponse> GetAudioStreamMonitoringInfoAsync(Guid hearingId)
        {
            try
            {
                var tasks = _wowzaClients
                    .Select(x => x.MonitoringStreamRecorderAsync(hearingId.ToString(), _configuration.ServerName, _configuration.HostName))
                    .ToList();

                var response = await WaitAnyFirstValidResult(tasks);

                _logger.LogInformation("Got Wowza monitor stream data for application: {hearingId}", hearingId);

                return response;
            }
            catch (AudioPlatformException ex)
            {
                var errorMessageTemplate = "Failed to get Wowza monitor stream data for application {hearingId}, StatusCode: {ex.StatusCode}, Error: {ex.Message}";
                LogError(ex, errorMessageTemplate, hearingId, ex.StatusCode, ex.Message);
                return null;
            }
        }

        public async Task<WowzaGetStreamRecorderResponse> GetAudioStreamInfoAsync(Guid hearingId)
        {
            try
            {
                var tasks = _wowzaClients
                    .Select(x => x.GetStreamRecorderAsync(hearingId.ToString(), _configuration.ServerName, _configuration.HostName))
                    .ToList();

                var response = await WaitAnyFirstValidResult(tasks);

                _logger.LogInformation("Got Wowza stream recorder for application: {hearingId}", hearingId);

                return response;
            }
            catch (AudioPlatformException ex)
            {
                var errorMessageTemplate = "Failed to get the Wowza stream recorder for application: {hearingId}, StatusCode: {ex.StatusCode}, Error: {ex.Message}";                
                LogError(ex, errorMessageTemplate, hearingId, ex.StatusCode, ex.Message);
                return null;
            }
        }

        public async Task<AudioPlatformServiceResponse> DeleteAudioStreamAsync(Guid hearingId)
        {
            try
            {
                var tasks = _wowzaClients.Select(x => x.StopStreamRecorderAsync(hearingId.ToString(), _configuration.ServerName, _configuration.HostName));
                await Task.WhenAll(tasks);

                _logger.LogInformation("Stopped Wowza stream recorder for application: {hearingId}", hearingId);

                return new AudioPlatformServiceResponse(true);
            }
            catch (AudioPlatformException ex)
            {
                var errorMessageTemplate = "Failed to get the Wowza stream recorder for application: {hearingId}, StatusCode: {ex.StatusCode}, Error: {ex.Message}";
                var errorMessage = $"Failed to get the Wowza stream recorder for application: {hearingId}, StatusCode: {ex.StatusCode}, Error: {ex.Message}";
                LogError(ex, errorMessageTemplate, hearingId, ex.StatusCode, ex.Message);
                return new AudioPlatformServiceResponse(false){ Message = errorMessage, StatusCode = ex.StatusCode };
            }
        }

        public async Task<IEnumerable<WowzaGetDiagnosticsResponse>> GetDiagnosticsAsync()
        {
            try
            {
                var tasks = _wowzaClients
                    .Select(x => x.GetDiagnosticsAsync(_configuration.ServerName))
                    .ToList();

                var response = await Task.WhenAll(tasks);
                
                return response;
            }
            catch (AudioPlatformException ex)
            {
                var errorMessageTemplate = "Failed to get the Wowza server version for application: {_configuration.ServerName}, StatusCode: {ex.StatusCode}, Error: {ex.Message}";
                LogError(ex, errorMessageTemplate, _configuration.ServerName, ex.StatusCode, ex.Message);
                return null;
            }
        }

        private async Task CreateAndUpdateApplicationAsync(string applicationName)
        {
            foreach (var client in _wowzaClients)
            {
                await client.CreateApplicationAsync(applicationName, _configuration.ServerName, _configuration.HostName, _configuration.StorageDirectory);
                _logger.LogInformation("Created a Wowza application for: {applicationName}", applicationName);

                await client.UpdateApplicationAsync(applicationName, _configuration.ServerName, _configuration.HostName, _configuration.AzureStorageDirectory);
                _logger.LogInformation("Updating Wowza application for: {applicationName}", applicationName);
            }
            
        }

        private void LogError(AudioPlatformException ex, string errorMessageTemplate, params object[] args)
        {
            if (ex.StatusCode != HttpStatusCode.NotFound)
            {
                _logger.LogError(ex, errorMessageTemplate, args);
            }
        }

        private string GetAudioIngestUrl(string applicationName) => $"{_configuration.StreamingEndpoint}{applicationName}/{applicationName}";

        private static async Task<T> WaitAnyFirstValidResult<T>(List<Task<T>> tasks)
        {
            while (tasks.Any())
            {
                var task = await Task.WhenAny(tasks);

                if (task != null && !task.IsCanceled && !task.IsFaulted)
                {
                    return await task;
                }

                tasks.Remove(task);
            }

            return default;
        }
    }
}
