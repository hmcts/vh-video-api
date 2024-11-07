using System.Net.Http;
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

        private static readonly string[] Roles = ["ROLE_ADMIN", "ROLE_GENTLEMAN"];

        public HttpClient CreateClient()
        {
            var client = Server.CreateClient();
            client.SetFakeBearerToken("admin", Roles);
            return client;
        }
    }
}
