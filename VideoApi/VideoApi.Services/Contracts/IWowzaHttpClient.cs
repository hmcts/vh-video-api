using System.Threading.Tasks;
using VideoApi.Services.Responses;

namespace VideoApi.Services.Contracts
{
    public interface IWowzaHttpClient
    {
        Task CreateApplicationAsync(string applicationName, string server, string host, string storageDirectory);
        Task UpdateApplicationAsync(string applicationName, string server, string host, string azureStorageDirectory);
        Task DeleteApplicationAsync(string applicationName, string server, string host);
        Task<WowzaMonitorStreamResponse> MonitoringStreamRecorderAsync(string applicationName, string server, string host);
        Task<WowzaGetApplicationResponse> GetApplicationAsync(string applicationName, string server, string host);
        Task<WowzaGetStreamRecorderResponse> GetStreamRecorderAsync(string applicationName, string server, string host); 
        Task StopStreamRecorderAsync(string applicationName, string server, string host);
        Task<WowzaGetDiagnosticsResponse> GetDiagnosticsAsync(string server);
    }
}
