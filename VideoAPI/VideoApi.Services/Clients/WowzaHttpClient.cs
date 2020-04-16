using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VideoApi.Common.Configuration;
using VideoApi.Services.Exceptions;

namespace VideoApi.Services.Clients
{
    public interface IWowzaHttpClient
    {
        Task CreateApplicationAsync(string applicationName);
        Task AddStreamRecorderAsync();
        Task MonitoringStreamRecorderAsync();
        Task GetApplicationsAsync();
        Task StopStreamRecorderAsync();   
    }
    
    public class WowzaHttpClient : IWowzaHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly StreamingConfiguration _configuration;

        public WowzaHttpClient(HttpClient httpClient, StreamingConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }
        
        public async Task CreateApplicationAsync(string applicationName)
        {
            var request = new CreateApplicationRequest
            {
                AppType = "Live",
                Name = applicationName,
                ClientStreamReadAccess = "*",
                ClientStreamWriteAccess = "*",
                Description = "Video Hearings Application for Audio Recordings",
                StreamConfig = new StreamConfigurationConfig
                {
                    CreateStorageDir = true,
                    StreamType = "live-record",
                    StorageDir = $"{_configuration.StorageDirectory}{applicationName}",
                    StorageDirExists = false
                },
                SecurityConfig = new SecurityConfigRequest
                {
                    PublishBlockDuplicateStreamNames = true,
                    PublishIPWhiteList = "*", 
                }
            };

            var response = await _httpClient.PostAsync
            (
                new Uri($"v2/servers/{_configuration.ServerName}/vhosts/{_configuration.HostName}/applications"), 
                new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
            );

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                
                throw new StreamingEngineException(errorMessage, response.StatusCode);
            }
        }

        public Task AddStreamRecorderAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task MonitoringStreamRecorderAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task GetApplicationsAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task StopStreamRecorderAsync()
        {
            throw new System.NotImplementedException();
        }
        
        private class CreateApplicationRequest
        {
            public string AppType { get; set; }
            public string Name { get; set; }
            public StreamConfigurationConfig StreamConfig { get; set; }
            public string ClientStreamWriteAccess { get; set; }
            public string ClientStreamReadAccess { get; set; }
            public string Description { get; set; }
            public SecurityConfigRequest SecurityConfig { get; set; }
        }
        
        private class AddStreamRecorderRequest
        {
            public string RecorderName { get; set; }
            public int CurrentSize { get; set; }
            public bool StartOnKeyFrame { get; set; }
            public bool RecordData { get; set; }
            public string OutputPath { get; set; }
            public string CurrentFile { get; set; }
            public string SegmentationType { get; set; }
            public string FileFormat { get; set; }
            public string RecorderState { get; set; }
            public string Option { get; set; }
            public bool DefaultRecorder { get; set; }
            public bool SplitOnTcDiscontinuity { get; set; }
            public string VersioningOption { get; set; }
            public string OutputFile { get; set; }
            public string FileTemplate { get; set; }
            public bool DefaultAudioSearchPosition { get; set; }
        }
        
        private class StreamConfigurationConfig
        {
            public bool StorageDirExists { get; set; }
            public bool CreateStorageDir { get; set; }
            public string StreamType { get; set; }
            public string StorageDir { get; set; }
        }
    
        private class SecurityConfigRequest
        {
            /// <summary>
            /// Comma separated string
            /// </summary>
            public string PublishIPWhiteList { get; set; }
            public string PublishAuthenticationMethod { get; set; }
            public bool PublishBlockDuplicateStreamNames { get; set; }
            public string PublishRTMPSecureURL { get; set; }
        }
    }
}
