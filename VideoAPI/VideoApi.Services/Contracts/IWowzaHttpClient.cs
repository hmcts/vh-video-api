using System.Threading.Tasks;
using VideoApi.Services.Responses;

namespace VideoApi.Services.Contracts
{
    public interface IWowzaHttpClient
    {
        Task CreateApplicationAsync(string applicationName, string storageDirectory);
        Task UpdateApplicationAsync(string applicationName, string azureStorageDirectory);
        Task DeleteApplicationAsync(string applicationName);
        Task AddStreamRecorderAsync(string applicationName);
        Task<WowzaMonitorStreamResponse> MonitoringStreamRecorderAsync(string applicationName);
        Task<WowzaGetApplicationResponse> GetApplicationAsync(string applicationName);
        Task<WowzaGetStreamRecorderResponse> GetStreamRecorderAsync(string applicationName); 
        Task StopStreamRecorderAsync(string applicationName);
        Task<WowzaGetDiagnosticsResponse> GetDiagnosticsAsync();
    }
}
