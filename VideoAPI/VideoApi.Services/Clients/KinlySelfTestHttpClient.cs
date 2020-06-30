using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VideoApi.Common.Configuration;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services.Contracts;
using VideoApi.Services.Helpers;
using VideoApi.Services.Kinly;

namespace VideoApi.Services.Clients
{
    public class KinlySelfTestHttpClient : IKinlySelfTestHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ServicesConfiguration _servicesConfigOptions;
        private readonly ILogger<KinlySelfTestHttpClient> _logger;

        public KinlySelfTestHttpClient(HttpClient httpClient, IOptions<ServicesConfiguration> servicesConfigOptions, ILogger<KinlySelfTestHttpClient> logger)
        {
            _httpClient = httpClient;
            _servicesConfigOptions = servicesConfigOptions.Value;
            _logger = logger;
        }

        public async Task<TestCallResult> GetTestCallScoreAsync(Guid participantId)
        {
            _logger.LogInformation($"Retrieving test call score for participant {participantId} at {_servicesConfigOptions.KinlySelfTestApiUrl}");
            
            var requestUri = $"{_servicesConfigOptions.KinlySelfTestApiUrl}/testcall/{participantId}";
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(requestUri),
                Method = HttpMethod.Get,
                Properties = {{"participantId", participantId}}
            };

            var responseMessage = await _httpClient.SendAsync(request);

            if (responseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning($" {responseMessage.StatusCode} : Failed to retrieve self test score for participant {participantId} ");
                return null;
            }

            var content = await responseMessage.Content.ReadAsStringAsync();
            var testCall = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<Testcall>(content);
            _logger.LogWarning($" {responseMessage.StatusCode} : Successfully retrieved self test score for participant {participantId} ");
            
            return new TestCallResult(testCall.Passed, (TestScore) testCall.Score);
        }
    }
}
