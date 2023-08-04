using System.Net.Http;
using AcceptanceTests.Common.Api;
using GST.Fake.Authentication.JwtBearer;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Testing.Common;
using Testing.Common.Configuration;
using VideoApi.DAL;
using VideoApi.IntegrationTests.Helper;

namespace VideoApi.IntegrationTests.Contexts
{
    public class TestContext
    {
        public Config Config { get; set; }
        public HttpContent HttpContent { get; set; }
        public HttpMethod HttpMethod { get; set; }
        public HttpResponseMessage Response { get; set; }
        public TestServer Server { get; set; }
        public Test Test { get; set; }
        public TestDataManager TestDataManager { get; set; }
        public VideoApiTokens Tokens { get; set; }
        public string Uri { get; set; }
        public DbContextOptions<VideoApiDbContext> VideoBookingsDbContextOptions { get; set; }
        public AzureStorageManager AzureStorage { get; set; }

        public HttpClient CreateClient()
        {
            HttpClient client;
            if (Zap.SetupProxy)
            {
                var handler = new HttpClientHandler
                {
                    Proxy = Zap.WebProxy,
                    UseProxy = true,
                };

                client = new HttpClient(handler)
                {
                    BaseAddress = new System.Uri(Config.Services.VideoApiUrl)
                };
            }
            else
            {
                client = Server.CreateClient();
            }
            
            client.SetFakeBearerToken("admin", new[] { "ROLE_ADMIN", "ROLE_GENTLEMAN" });
            return client;
        }
    }
}
