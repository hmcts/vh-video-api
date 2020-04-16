﻿using System.Threading.Tasks;
using VideoApi.Services.Responses;

namespace VideoApi.Services.Contracts
{
    public interface IWowzaHttpClient
    {
        Task CreateApplicationAsync(string applicationName, string server, string host, string storageDirectory);
        Task AddStreamRecorderAsync(string applicationName, string server, string host);
        Task<WowzaMonitorStreamResponse> MonitoringStreamRecorderAsync(string applicationName, string server, string host);
        Task<WowzaGetApplicationsResponse> GetApplicationsAsync(string server, string host);
        Task<WowzaGetApplicationResponse> GetApplicationAsync(string applicationName, string server, string host);
        Task StopStreamRecorderAsync(string applicationName, string server, string host); 
    }
}
