using System.Threading.Tasks;
using VideoApi.Services.Responses;

namespace VideoApi.Services.Contracts
{
    public interface IWowzaHttpClient
    {
        Task CreateApplicationAsync(string applicationName);
        Task AddStreamRecorderAsync(string applicationName);
        Task<WowzaMonitorStreamResponse> MonitoringStreamRecorderAsync(string applicationName);
        Task<WowzaGetApplicationsResponse> GetApplicationsAsync();
        Task<WowzaGetApplicationResponse> GetApplicationAsync(string applicationName);
        Task StopStreamRecorderAsync(string applicationName); 
    }
}
