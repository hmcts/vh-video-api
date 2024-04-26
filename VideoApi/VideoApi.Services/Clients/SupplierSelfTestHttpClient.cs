using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VideoApi.Common.Security.Supplier.Kinly;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services.Contracts;
using VideoApi.Services.Helpers;

namespace VideoApi.Services.Clients
{
    public class SupplierSelfTestHttpClient : ISupplierSelfTestHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly KinlyConfiguration _kinlyConfigOptions;
        private readonly ILogger<SupplierSelfTestHttpClient> _logger;

        public SupplierSelfTestHttpClient(HttpClient httpClient, IOptions<KinlyConfiguration> kinlyConfigOptions, ILogger<SupplierSelfTestHttpClient> logger)
        {
            _httpClient = httpClient;
            _kinlyConfigOptions = kinlyConfigOptions.Value;
            _logger = logger;
        }

        public async Task<TestCallResult> GetTestCallScoreAsync(Guid participantId)
        {
            _logger.LogInformation("Retrieving test call score for participant {participantId} at {SelfTestApiUrl}", participantId, _kinlyConfigOptions.SelfTestApiUrl);
            
            var requestUri = $"{_kinlyConfigOptions.SelfTestApiUrl}/testcall/{participantId}";
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(requestUri),
                Method = HttpMethod.Get,
                Properties = {{"participantId", participantId}}
            };

            var responseMessage = await _httpClient.SendAsync(request);

            if (responseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning(" {StatusCode} : Failed to retrieve self test score for participant {participantId}", responseMessage.StatusCode, participantId);
                return null;
            }

            var content = await responseMessage.Content.ReadAsStringAsync();
            var testCall = ApiRequestHelper.Deserialise<Testcall>(content);
            _logger.LogWarning(" {StatusCode} : Successfully retrieved self test score for participant {participantId}", responseMessage.StatusCode, participantId);
            
            return new TestCallResult(testCall.Passed, (TestScore) testCall.Score);
        }
    }
}
