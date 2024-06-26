using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VideoApi.Common.Helpers;
using VideoApi.Services.Contracts;
using VideoApi.Services.Exceptions;
using VideoApi.Services.Responses;

namespace VideoApi.Services.Clients
{
    public class WowzaHttpClient : IWowzaHttpClient
    {
        private readonly HttpClient _httpClient;

        public WowzaHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public bool IsLoadBalancer { get; set; }
        
        public async Task CreateApplicationAsync(string applicationName, string server, string host, string storageDirectory)
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
                    StreamType = "live",
                    StorageDir = $"{storageDirectory}{applicationName}",
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
                $"v2/servers/{server}/vhosts/{host}/applications",
                new StringContent(ApiRequestHelper.SerialiseRequestToCamelCaseJson(request), Encoding.UTF8, "application/json")
            );

            await HandleUnsuccessfulResponse(response);
        }

        public async Task UpdateApplicationAsync(string applicationName, string server, string host, string azureStorageDirectory)
        {
            var request = new ApplicationConfigAdvRequest
            {
                Modules = new[]
                {
                    new ModuleConfig
                    {
                        Name = "base",
                        Description = "Base",
                        Class = "com.wowza.wms.module.ModuleCore",
                        Order = 0
                    },
                    new ModuleConfig
                    {
                        Name = "logging",
                        Description = "Client Logging",
                        Class = "com.wowza.wms.module.ModuleClientLogging",
                        Order = 1
                    },
                    new ModuleConfig
                    {
                        Name = "flvplayback",
                        Description = "FLVPlayback12",
                        Class = "com.wowza.wms.module.ModuleFLVPlayback",
                        Order = 2
                    },
                    new ModuleConfig
                    {
                        Name = "ModuleCoreSecurity",
                        Description = "Core Security Module for Applications",
                        Class = "com.wowza.wms.module.ModuleCoreSecurity",
                        Order = 3
                    },
                    new ModuleConfig
                    {
                        Name = "ModuleMediaWriterFileMover",
                        Description = "ModuleMediaWriterFileMover",
                        Class = "com.wowza.wms.module.ModuleMediaWriterFileMover",
                        Order = 4
                    },
                    new ModuleConfig
                    {
                        Name = "ModuleAutoRecord",
                        Description = "ModuleAutoRecord",
                        Class = "com.wowza.wms.plugin.ModuleAutoRecord",
                        Order = 5
                    }
                },
                AdvancedSettings = new[]
                {
                    new AdvancedSetting
                    {
                        SectionName = "Application",
                        Section = "/Root/Application",
                        Name = "fileMoverDestinationPath",
                        Type = "String",
                        Value = azureStorageDirectory,
                        Documented = false,
                        Enabled = true
                    },
                    new AdvancedSetting
                    {
                        SectionName = "Application",
                        Section = "/Root/Application",
                        Name = "fileMoverDeleteOriginal",
                        Type = "Boolean",
                        Value = "false",
                        Documented = false,
                        Enabled = true
                    },
                    new AdvancedSetting
                    {
                        SectionName = "Application",
                        Section = "/Root/Application",
                        Name = "fileMoverVersionFile",
                        Type = "Boolean",
                        Value = "false",
                        Documented = false,
                        Enabled = true
                    }
                }
            };

            var response = await _httpClient.PostAsync
            (
                $"v2/servers/{server}/vhosts/{host}/applications/{applicationName}/adv",
                new StringContent(ApiRequestHelper.SerialiseRequestToCamelCaseJson(request), Encoding.UTF8, "application/json")
            );

            await HandleUnsuccessfulResponse(response);
        }

        public async Task DeleteApplicationAsync(string applicationName, string server, string host)
        {
            var response = await _httpClient.DeleteAsync
            (
                $"v2/servers/{server}/vhosts/{host}/applications/{applicationName}"
            );

            await HandleUnsuccessfulResponse(response);
        }

        public async Task AddStreamRecorderAsync(string applicationName, string server, string host)
        {
            var request = new AddStreamRecorderRequest
            {
                RecorderName = applicationName,
                StartOnKeyFrame = true,
                DefaultRecorder = true,
                SplitOnTcDiscontinuity = false,
                OutputPath = "",
                CurrentFile = "",
                SegmentationType = "SEGMENT_NONE",
                FileFormat = "MP4",
                RecorderState = "",
                Option = "APPEND_FILE"
            };

            var response = await _httpClient.PostAsync
            (
                $"v2/servers/{server}/vhosts/{host}/applications/" +
                $"{applicationName}/instances/_definst_/streamrecorders",
                new StringContent(ApiRequestHelper.SerialiseRequestToCamelCaseJson(request), Encoding.UTF8, "application/json")
            );

            await HandleUnsuccessfulResponse(response);
        }

        public async Task<WowzaMonitorStreamResponse> MonitoringStreamRecorderAsync(string applicationName, string server, string host, string hearingId)
        {
            var response = await _httpClient.GetAsync
            (
                $"v2/servers/{server}/vhosts/{host}/applications/" +
                $"{applicationName}/instances/_definst_/incomingstreams/{hearingId}/monitoring/current"
            );

            await HandleUnsuccessfulResponse(response);

            return JsonConvert.DeserializeObject<WowzaMonitorStreamResponse>(await response.Content.ReadAsStringAsync());
        }

        public async Task<WowzaGetApplicationResponse> GetApplicationAsync(string applicationName, string server, string host)
        {
            var response = await _httpClient.GetAsync
            (
                $"v2/servers/{server}/vhosts/{host}/applications/{applicationName}"
            );

            await HandleUnsuccessfulResponse(response);

            return JsonConvert.DeserializeObject<WowzaGetApplicationResponse>(await response.Content.ReadAsStringAsync());
        }

        public async Task<HttpResponseMessage> GetStreamRecorderAsync(string applicationName, string server, string host, string recorder)
        {
            var requestUrl = $"v2/servers/{server}/vhosts/{host}/applications/{applicationName}/instances/_definst_/streamrecorders/{recorder}";
            using var cts = new CancellationTokenSource(new TimeSpan(0, 0, 10));
            return await _httpClient.GetAsync(requestUrl, cts.Token).ConfigureAwait(false);
        }

        public async Task StopStreamRecorderAsync(string applicationName, string server, string host, string hearingId)
        {
            var response = await _httpClient.PutAsync
            (
                $"v2/servers/{server}/vhosts/{host}/applications/" +
                $"{applicationName}/instances/_definst_/streamrecorders/{hearingId}/actions/stopRecording",
                new StringContent("")
            );

            await HandleUnsuccessfulResponse(response);
        }

        public async Task<HttpResponseMessage> GetDiagnosticsAsync(string server)
        {
            using var cts = new CancellationTokenSource(new TimeSpan(0, 0, 2));
            var response = await _httpClient.GetAsync
            (
                $"v2/servers/{server}/status",
                cts.Token
            );
            
            await HandleUnsuccessfulResponse(response);
            
            return response;
        }

        private static async Task HandleUnsuccessfulResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();

                throw new AudioPlatformException(errorMessage, response.StatusCode);
            }
        }

        private sealed class CreateApplicationRequest
        {
            public string AppType { get; set; }
            public string Name { get; set; }
            public StreamConfigurationConfig StreamConfig { get; set; }
            public string ClientStreamWriteAccess { get; set; }
            public string ClientStreamReadAccess { get; set; }
            public string Description { get; set; }
            public SecurityConfigRequest SecurityConfig { get; set; }
        }

        private sealed class AddStreamRecorderRequest
        {
            public string RecorderName { get; set; }
            public bool StartOnKeyFrame { get; set; }
            public string OutputPath { get; set; }
            public string CurrentFile { get; set; }
            public string SegmentationType { get; set; }
            public string FileFormat { get; set; }
            public string RecorderState { get; set; }
            public string Option { get; set; }
            public bool DefaultRecorder { get; set; }
            public bool SplitOnTcDiscontinuity { get; set; }
        }

        private sealed class StreamConfigurationConfig
        {
            public bool StorageDirExists { get; set; }
            public bool CreateStorageDir { get; set; }
            public string StreamType { get; set; }
            public string StorageDir { get; set; }
        }

        private sealed class SecurityConfigRequest
        {
            /// <summary>
            /// Comma separated string
            /// </summary>
            public string PublishIPWhiteList { get; set; }

            public bool PublishBlockDuplicateStreamNames { get; set; }
        }

        private sealed class ApplicationConfigAdvRequest
        {
            public AdvancedSetting[] AdvancedSettings { get; set; }
            public ModuleConfig[] Modules { get; set; }
        }

        private sealed class AdvancedSetting
        {
            public string SectionName { get; set; }
            public string Section { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string Value { get; set; }
            public bool Documented { get; set; }
            public bool Enabled { get; set; }
        }

        private sealed class ModuleConfig
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Class { get; set; }
            public int Order { get; set; }
        }
    }
}
