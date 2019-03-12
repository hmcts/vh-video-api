using System;
using System.Net.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL;
using VideoApi.IntegrationTests.Helper;

namespace VideoApi.IntegrationTests.Contexts
{
    public class ApiTestContext
    {
        public DbContextOptions<VideoApiDbContext> VideoBookingsDbContextOptions { get; set; }
        public TestDataManager TestDataManager { get; set; }
        public TestServer Server { get; set; }
        public string DbString { get; set; }
        public string BearerToken { get; set; }
        public string Uri { get; set; }
        public HttpMethod HttpMethod { get; set; }
        public StringContent StringContent { get; set; }
        public HttpContent HttpContent { get; set; }
        public Guid NewConferenceId { get; set; }
        public HttpResponseMessage ResponseMessage { get; set; }
    }
}