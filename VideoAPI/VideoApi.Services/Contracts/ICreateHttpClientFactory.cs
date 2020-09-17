using System.Collections.Generic;
using System.Net.Http;

namespace VideoApi.Services.Contracts
{
    public interface ICreateHttpClientFactory
    {
        List<WowzaClientModel> GetHttpClients();
    }

    public class WowzaClientModel
    {
        public HttpClient HttpClientForNode { get; set; }
        public string ServerName { get; set; }
        public string HostName { get; set; }
    }
}
