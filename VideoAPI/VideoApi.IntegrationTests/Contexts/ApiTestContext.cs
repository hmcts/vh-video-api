using System;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Testing.Common.Configuration;
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
                return ConfigurationRoot.GetSection("IntegrationTestSettings").Get<TestConfiguration>();
            }
        }

        private ZapConfiguration ZapConfiguration
        {
            get
            {
                return ConfigurationRoot.GetSection("ZapConfiguration").Get<ZapConfiguration>();
            }
        }

        private IConfigurationRoot ConfigurationRoot => new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        public HttpClient CreateClient()
        {
            HttpClient client;
            if (ZapConfiguration.RunZap)
            {
                var handler = new HttpClientHandler
                {
                    Proxy = Zap.WebProxy,
                    UseProxy = true,
                };

                client = new HttpClient(handler)
                {
                    BaseAddress = new System.Uri(TestConfiguration.VideoApiBaseUrl)
                };
            }
            else
            {
                client = Server.CreateClient();
            }
            
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {BearerToken}");
            return client;
        }
    }
}
