using VideoApi.Contract.Responses;
using VideoApi.Services.Responses;

namespace Video.API.Mappings
{
    public static class AudioRecordingMapper
    {
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
    }
}
