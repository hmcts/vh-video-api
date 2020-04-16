using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VideoApi.Services.Clients;
using VideoApi.Services.Contracts;
using VideoApi.Services.Exceptions;

namespace VideoApi.Services
{
    public class WowzaStreamingService : IConferenceStreamingService
    {
        private readonly IWowzaHttpClient _wowzaClient;
        private readonly ILogger<WowzaStreamingService> _logger;

        public WowzaStreamingService(IWowzaHttpClient wowzaClient, ILogger<WowzaStreamingService> logger)
        {
            _wowzaClient = wowzaClient;
            _logger = logger;
        }

        public async Task CreateConferenceStreamAsync(string caseNumber, Guid conferenceId)
        {
            var applicationName = $"{caseNumber}_{conferenceId}";
            
            try
            {
                await _wowzaClient.CreateApplicationAsync(applicationName);
            }
            catch (StreamingEngineException ex)
            {
                _logger.LogError($"Failed to create the Wowza Application: {applicationName}, StatusCode: {ex.StatusCode}", ex);
            }   
        }
    }
}
