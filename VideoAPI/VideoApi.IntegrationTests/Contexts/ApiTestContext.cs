using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Testing.Common.Helper;
using VideoApi.Common.Configuration;
using VideoApi.Common.Security.CustomToken;
using VideoApi.DAL;
using VideoApi.IntegrationTests.Helper;
using VideoApi.IntegrationTests.Helpers;

namespace VideoApi.IntegrationTests.Contexts
{
    public class ApiTestContext
    {
        public DbContextOptions<VideoApiDbContext> VideoBookingsDbContextOptions { get; set; }
        public TestDataManager TestDataManager { get; set; }
        public ServicesConfiguration Services { get; set; }
        public TestServer Server { get; set; }
        public string DbString { get; set; }
        public string BearerToken { get; set; }
        public string Uri { get; set; }
        public HttpMethod HttpMethod { get; set; }
        public StringContent StringContent { get; set; }
        public HttpContent HttpContent { get; set; }
        public HttpRequest HttRequest { get; set; }
        public Guid NewConferenceId { get; set; }
        public HttpResponseMessage ResponseMessage { get; set; }
        public CustomTokenSettings CustomTokenSettings { get; set; }
        public string RequestUrl => TestConfiguration.VideoApiBaseUrl + Uri;

        private TestConfiguration TestConfiguration
        {
            get
            {
                return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("IntegrationTestSettings").Get<TestConfiguration>();
            }
        }

        public HttpClient CreateClient()
        {
            var handler = new HttpClientHandler
            {
                Proxy = ZAP.WebProxy,
                UseProxy = true,
            };

            var client = new HttpClient(handler)
            {
                BaseAddress = new System.Uri(TestConfiguration.VideoApiBaseUrl)
            };
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {this.BearerToken}");
            return client;
        }
    }
}
