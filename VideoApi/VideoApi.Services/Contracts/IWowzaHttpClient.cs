using System.Net.Http;
using System.Threading.Tasks;
using VideoApi.Services.Responses;

namespace VideoApi.Services.Contracts
{
    public interface IWowzaHttpClient
    {
        public bool IsLoadBalancer { get; set; }
        Task DeleteApplicationAsync(string applicationName, string server, string host);
        Task<WowzaMonitorStreamResponse> MonitoringStreamRecorderAsync(string applicationName, string server, string host, string hearingId);
        Task<WowzaGetApplicationResponse> GetApplicationAsync(string applicationName, string server, string host);
        Task<WowzaGetStreamRecorderResponse> GetStreamRecorderAsync(string applicationName, string server, string host, string hearingId); 
        Task StopStreamRecorderAsync(string applicationName, string server, string host, string hearingId);
        Task<HttpResponseMessage> GetDiagnosticsAsync(string server);
    }
}
