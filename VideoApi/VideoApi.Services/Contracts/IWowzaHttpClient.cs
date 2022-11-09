using System.Net.Http;
using System.Threading.Tasks;
using VideoApi.Services.Responses;

namespace VideoApi.Services.Contracts
{
    public interface IWowzaHttpClient
    {
        public bool IsLoadBalancer { get; }
        Task DeleteApplicationAsync(string applicationName, string server, string host);
        Task<WowzaMonitorStreamResponse> MonitoringStreamRecorderAsync(string applicationName, string server, string host, string hearingId);
        Task<WowzaGetApplicationResponse> GetApplicationAsync(string applicationName, string server, string host);
        Task<HttpResponseMessage> GetStreamRecorderAsync(string applicationName, string server, string host, string recorder); 
        Task StopStreamRecorderAsync(string applicationName, string server, string host, string hearingId);
        Task<HttpResponseMessage> GetDiagnosticsAsync(string server);
    }
}
