using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
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

        /// <summary>
        /// The function `GetAudioIngestUrl` generates a URL based on input parameters after sanitizing
        /// them by removing special characters.
        /// </summary>
        /// <param name="serviceId">The `serviceId` parameter is a unique identifier for a
        /// service.</param>
        /// <param name="caseNumber">The `caseNumber` parameter is a string that represents the case
        /// number associated with a particular legal case. It is used as part of the URL construction
        /// in the `GetAudioIngestUrl` method to uniquely identify the case for which the audio ingest
        /// URL is being generated.</param>
        /// <param name="hearingId">The `hearingId` parameter in the `GetAudioIngestUrl` method
        /// represents the unique identifier for a specific hearing. It is used to identify and retrieve
        /// the audio associated with that particular hearing.</param>
        /// <returns>
        /// The method `GetAudioIngestUrl` returns a string that concatenates the
        /// `_configuration.StreamingEndpoint`, `ApplicationName`, `sanitisedServiceId`,
        /// `sanitisedCaseNumber`, and `hearingId` with hyphens in between.
        /// </returns>
        public string GetAudioIngestUrl(string serviceId, string caseNumber, string hearingId)
        {
            const string regex = "[^a-zA-Z0-9]";
            const RegexOptions regexOptions = RegexOptions.None;
            var timeout = TimeSpan.FromMilliseconds(500);

            var sanitisedServiceId = Regex.Replace(serviceId, regex, "", regexOptions, timeout);
            var sanitisedCaseNumber = Regex.Replace(caseNumber, regex, "", regexOptions, timeout);
            
            return $"{_configuration.StreamingEndpoint}{ApplicationName}/{sanitisedServiceId}-{sanitisedCaseNumber}-{hearingId}";
        }
        
        public string ApplicationName { get; }
        
        /// <summary>
        /// This C# async function retrieves audio stream information from Wowza stream recorders using
        /// multiple clients and handles exceptions.
        /// </summary>
        /// <param name="recorder">The `recorder` parameter in the `GetAudioStreamInfoAsync` method is
        /// used to specify the name of the stream recorder for which you want to retrieve information.
        /// This method iterates through a list of Wowza clients to make a request to get stream
        /// recorder information for the specified `recorder</param>
        /// <returns>
        /// A `WowzaGetStreamRecorderResponse` object is being returned asynchronously.
        /// </returns>
        public async Task<WowzaGetStreamRecorderResponse> GetAudioStreamInfoAsync(string recorder)
        {
            var responses = new List<HttpResponseMessage>();
            foreach (var client in _wowzaClients)
            {
                try
                {
                    var response = await client.GetStreamRecorderAsync(ApplicationName, _configuration.ServerName, _configuration.HostName, recorder);

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("Got Wowza stream recorder for application: {recorder}", recorder);
                        return JsonSerializer.Deserialize<WowzaGetStreamRecorderResponse>(await response.Content.ReadAsStringAsync());
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

        /// <summary>
        /// The function `GetDiagnosticsAsync` asynchronously retrieves diagnostics from a server and
        /// logs any errors that occur.
        /// </summary>
        /// <returns>
        /// The method `GetDiagnosticsAsync` returns a `Task<bool>`. The method makes an asynchronous
        /// call to `_loadBalancerClient.GetDiagnosticsAsync` to get diagnostics for a server specified
        /// by `_configuration.ServerName`. If the call is successful (response has
        /// `IsSuccessStatusCode`), it returns `true`. If an `AudioPlatformException` is caught during
        /// the process, it logs an error
        /// </returns>
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

        /// <summary>
        /// This C# function asynchronously deletes a Wowza application using multiple clients and logs
        /// any errors that occur.
        /// </summary>
        /// <param name="Guid">A `Guid` is a globally unique identifier, which is a 128-bit integer
        /// often used in software development to uniquely identify resources or entities. In the
        /// context of the `DeleteAudioApplicationAsync` method you provided, the `Guid hearingId`
        /// parameter represents the unique identifier of a specific audio application</param>
        /// <returns>
        /// The method `DeleteAudioApplicationAsync` returns an `AudioPlatformServiceResponse` object.
        /// If the deletion of the Wowza application is successful, it returns a new
        /// `AudioPlatformServiceResponse` object with a `true` status. If an `AudioPlatformException`
        /// is caught during the deletion process, it returns a new `AudioPlatformServiceResponse`
        /// object with a `false` status, along
        /// </returns>
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
#pragma warning disable CA2254 // Template should be a static expression
                _logger.LogError(ex, errorMessageTemplate, args);
#pragma warning restore CA2254 // Template should be a static expression
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
            while (tasks.Count != 0)
            {
                var task = await Task.WhenAny(tasks);

                if (!task.IsCanceled && !task.IsFaulted)
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
