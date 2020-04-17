using VideoApi.Contract.Responses;
using VideoApi.Services.Responses;

namespace Video.API.Mappings
{
    public static class AudioRecordingMapper
    {
        public static AudioApplicationInfoResponse MapToAudioApplicationInfo(WowzaGetApplicationResponse response)
        {
            return new AudioApplicationInfoResponse
            {
                Description = response.Description,
                Name = response.Name,
                ServerName = response.ServerName,
                StreamConfig = MapToAudioApplicationStreamConfigResponse(response.StreamConfig)
            };
        }

        public static AudioStreamInfoResponse MapToAudioStreamInfo(WowzaGetStreamRecorderResponse response)
        {
            return new AudioStreamInfoResponse
            {
                Option = response.Option,
                ApplicationName = response.ApplicationName,
                BaseFile = response.BaseFile,
                CurrentDuration = response.CurrentDuration,
                CurrentFile = response.CurrentFile,
                CurrentSize = response.CurrentSize,
                FileFormat = response.FileFormat,
                InstanceName = response.InstanceName,
                OutputPath = response.OutputPath,
                RecorderName = response.RecorderName,
                RecorderState = response.RecorderState,
                SegmentDuration = response.SegmentDuration,
                ServerName = response.ServerName,
                RecorderErrorString = response.RecorderErrorString,
                RecordingStartTime = response.RecordingStartTime
            };
        }

        public static AudioStreamMonitoringInfo MapToAudioStreamMonitoringInfo(WowzaMonitorStreamResponse response)
        {
            return new AudioStreamMonitoringInfo
            {
                Name = response.Name,
                Uptime = response.Uptime,
                ApplicationInstance = response.ApplicationInstance,
                BytesIn = response.BytesIn,
                ServerName = response.ServerName,
                BytesInRate = response.BytesInRate
            };
        }

        private static AudioApplicationStreamConfigResponse MapToAudioApplicationStreamConfigResponse(Streamconfig response)
        {
            if (response == null) return null;
            
            return new AudioApplicationStreamConfigResponse
            {
                KeyDir = response.KeyDir,
                ServerName = response.ServerName,
                StorageDir = response.StorageDir,
                StreamType = response.StreamType,
                StorageDirExists = response.StorageDirExists
            };
        }
    }
}
