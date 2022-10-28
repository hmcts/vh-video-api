using System.Threading.Tasks;
using VideoApi.Services.Responses;

namespace VideoApi.Services.Contracts
{
    public interface IWowzaHttpClient
    {
        Task DeleteApplicationAsync(string applicationName, string server, string host);
        Task<WowzaMonitorStreamResponse> MonitoringStreamRecorderAsync(string applicationName, string server, string host, string hearingId);
        Task<WowzaGetApplicationResponse> GetApplicationAsync(string applicationName, string server, string host);
        Task<WowzaGetStreamRecorderResponse> GetStreamRecorderAsync(string applicationName, string server, string host, string hearingId); 
        Task StopStreamRecorderAsync(string applicationName, string server, string host, string hearingId);
        Task<WowzaGetDiagnosticsResponse> GetDiagnosticsAsync(string server);
    }
}
