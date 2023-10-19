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

        public AudioPlatformService(IEnumerable<IWowzaHttpClient> wowzaClients, WowzaConfiguration configuration, ILogger<AudioPlatformService> logger)
        {
            _wowzaClients       = wowzaClients.Where(e => !e.IsLoadBalancer).ToArray();
            _loadBalancerClient = wowzaClients.FirstOrDefault(e => e.IsLoadBalancer);
            _configuration      = configuration;
            _logger             = logger;
            ApplicationName     = configuration.ApplicationName;
        }

        public string GetAudioIngestUrl(string hearingId) => $"{_configuration.StreamingEndpoint}{ApplicationName}/{hearingId}";

        public string GetAudioIngestUrl(string serviceId, string caseNumber, string hearingId) => 
            $"{_configuration.StreamingEndpoint}{ApplicationName}/{serviceId}-{caseNumber}-{hearingId}";
        
        public string ApplicationName { get; }
        
        public async Task<WowzaGetStreamRecorderResponse> GetAudioStreamInfoAsync(string application, string recorder)
        {
            var responses = new List<HttpResponseMessage>();
            foreach (var client in _wowzaClients)
            {
                try
                {
                    var response = await client.GetStreamRecorderAsync(application, _configuration.ServerName, _configuration.HostName, recorder);

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("Got Wowza stream recorder for application: {recorder}", recorder);
                        return JsonConvert.DeserializeObject<WowzaGetStreamRecorderResponse>(await response.Content.ReadAsStringAsync());
                    }
                    responses.Add(response);
                }
                catch(Exception ex)
                {
                    responses.Add(new HttpResponseMessage(HttpStatusCode.InternalServerError){Content = new StringContent(ex.Message)});
                }
            }
            throw await GetAudioStreamExceptions(recorder, responses);
        }

        private async Task<AggregateException> GetAudioStreamExceptions(string recorder, List<HttpResponseMessage> responses)
        {
            const string errorMessageTemplate = "Failed to get the Wowza stream recorder for: {recorder}, StatusCode: {ex.StatusCode}, Error: {ex.Message}";
            var innerExceptions = new List<AudioPlatformException>();
            foreach (var response in responses)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                var exception = 
                    new AudioPlatformException(String.IsNullOrEmpty(errorMessage) ? "Unexpected exception" : errorMessage, response.StatusCode);
                LogError(exception, errorMessageTemplate, recorder, exception.StatusCode, exception.Message);
                innerExceptions.Add(exception); 
            }
            return new AggregateException(innerExceptions: innerExceptions);
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
        
        private void LogError(AudioPlatformException ex, string errorMessageTemplate, params object[] args)
        {
            if (ex.StatusCode != HttpStatusCode.NotFound)
            {
                _logger.LogError(ex, errorMessageTemplate, args);
            }
        }
        
        #region Obsolete
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
        #endregion
    }
}
