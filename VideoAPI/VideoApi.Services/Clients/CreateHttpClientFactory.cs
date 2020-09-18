using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using VideoApi.Common.Configuration;
using VideoApi.Services.Contracts;

namespace VideoApi.Services.Clients
{
    public class CreateHttpClientFactory : ICreateHttpClientFactory
    {
        private readonly WowzaConfiguration _configuration;
        public CreateHttpClientFactory(WowzaConfiguration configuration)
        {
            _configuration = configuration;
        }
        public List<WowzaClientModel> GetHttpClients()
        {
            var httpClients = new List<WowzaClientModel>();
            var restApiEndpoints = _configuration.RestApiEndpoint;
            foreach (var node in restApiEndpoints)
            {
                var httpClient = CreateClient(node);
                httpClients.Add(new WowzaClientModel
                { 
                    HostName = _configuration.HostName,
                    HttpClientForNode = httpClient,
                    ServerName = _configuration.ServerName
                });
            }
            return httpClients;
        }

        private HttpClient CreateClient(string restApiEndpoint)
        {
            var httpClientHandler = new HttpClientHandler
            {
                Credentials = new CredentialCache
                {
                    { new Uri(restApiEndpoint),
                    "Digest",
                    new NetworkCredential(_configuration.Username, _configuration.Password)
                    }
                }
            };

            var httpClient = new HttpClient(httpClientHandler)
            {
                BaseAddress = new Uri(restApiEndpoint),

            };
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("ContentType", "application/json");

            return httpClient;
        }

    }
  
}
