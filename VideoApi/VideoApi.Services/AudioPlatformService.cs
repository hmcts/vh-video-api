using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VideoApi.Common.Configuration;
using VideoApi.Contract.Responses;
using VideoApi.Services.Contracts;
using VideoApi.Services.Exceptions;
using VideoApi.Services.Responses;

namespace VideoApi.Services
{
    public class AudioPlatformService : IAudioPlatformService
    {
        private readonly IWowzaHttpClient[] _wowzaClients;
        private readonly WowzaConfiguration _configuration;
        private readonly ILogger<AudioPlatformService> _logger;
        private readonly IWowzaHttpClient _loadBalancerClient;

        public AudioPlatformService(IWowzaHttpClient[] wowzaClients, WowzaConfiguration configuration, ILogger<AudioPlatformService> logger)
        {
            _wowzaClients       = wowzaClients.Where(e => !e.IsLoadBalancer).ToArray();
            _loadBalancerClient = wowzaClients.First(e => e.IsLoadBalancer);
            _configuration      = configuration;
            _logger             = logger;
            ApplicationName     = configuration.ApplicationName;
        }

        public async Task<WowzaGetApplicationResponse> GetAudioApplicationInfoAsync(Guid? hearingId = null)
        {
            var applicationName = hearingId == null ? ApplicationName : hearingId.ToString();
            try
            {
                var tasks = _wowzaClients
                    .Select(x => x.GetApplicationAsync(applicationName, _configuration.ServerName, _configuration.HostName))
                    .ToList();

                var response = await WaitAnyFirstValidResult(tasks);
 
                _logger.LogInformation("Got Wowza application info: {AppName}", applicationName);

                return response;
            }
            catch (AudioPlatformException ex)
            {
                var errorMessageTemplate = "Failed to get info for Wowza application: {ApplicationName}, StatusCode: {ex.StatusCode}, Error: {ex.Message}";

                LogError(ex, errorMessageTemplate, applicationName, ex.StatusCode, ex.Message);

                return null;
            }
        }
        
        public async Task<WowzaMonitorStreamResponse> GetAudioStreamMonitoringInfoAsync(Guid hearingId)
        {
            try
            {
                var tasks = _wowzaClients
                    .Select(x => x.MonitoringStreamRecorderAsync(ApplicationName, _configuration.ServerName, _configuration.HostName, hearingId.ToString()))
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

        public async Task<WowzaGetStreamRecorderResponse> GetAudioStreamInfoAsync(string application, string recorder)
        {
            var responses = new List<HttpResponseMessage>();
            try
            {
                foreach (var client in _wowzaClients)
                    responses.Add(await client.GetStreamRecorderAsync(application, _configuration.ServerName, _configuration.HostName, recorder));
                responses.Remove(null);
                
                if (responses.All(e => !e.IsSuccessStatusCode)) 
                {
                    var errorMessage = await responses.First().Content.ReadAsStringAsync();
                    throw new AudioPlatformException(errorMessage, responses.First().StatusCode);
                }
                var successfulResponse = responses.First(e => e.IsSuccessStatusCode);
                _logger.LogInformation("Got Wowza stream recorder for application: {recorder}", recorder);
                return JsonConvert.DeserializeObject<WowzaGetStreamRecorderResponse>(await successfulResponse.Content.ReadAsStringAsync());
            }
            catch (AudioPlatformException ex)
            {
                var errorMessageTemplate = "Failed to get the Wowza stream recorder for: {recorder}, StatusCode: {ex.StatusCode}, Error: {ex.Message}";                
                LogError(ex, errorMessageTemplate, recorder, ex.StatusCode, ex.Message);
                return null;
            }
        }

        public async Task<AudioPlatformServiceResponse> DeleteAudioStreamAsync(Guid hearingId)
        {
            try
            {
                var tasks = _wowzaClients.Select(x => x.StopStreamRecorderAsync(ApplicationName, _configuration.ServerName, _configuration.HostName, hearingId.ToString()));
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

        public async Task<bool> GetDiagnosticsAsync()
        {
            try
            {
                var response = await _loadBalancerClient.GetDiagnosticsAsync(_configuration.ServerName);
                return response.IsSuccessStatusCode;
            }
            catch (AudioPlatformException ex)
            {
                var errorMessageTemplate = "Failed to get the Wowza server version for application: {_configuration.ServerName}, StatusCode: {ex.StatusCode}, Error: {ex.Message}";
                LogError(ex, errorMessageTemplate, _configuration.ServerName, ex.StatusCode, ex.Message);
                return false;
            }
        }
        private void LogError(AudioPlatformException ex, string errorMessageTemplate, params object[] args)
        {
            if (ex.StatusCode != HttpStatusCode.NotFound)
            {
                _logger.LogError(ex, errorMessageTemplate, args);
            }
        }

        public string GetAudioIngestUrl(string hearingId) => $"{_configuration.StreamingEndpoint}{ApplicationName}/{hearingId}";
        public string ApplicationName { get; }

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
    }
}
