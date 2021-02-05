namespace VideoApi.Contract.Responses
{
    public class AudioStreamInfoResponse
    {
        public string InstanceName { get; set; }
        public string ServerName { get; set; }
        public string RecorderName { get; set; }
        public int CurrentSize { get; set; }
        public string OutputPath { get; set; }
        public string CurrentFile { get; set; }
        public string ApplicationName { get; set; }
        public string RecorderErrorString { get; set; }
        public string BaseFile { get; set; }
        public int SegmentDuration { get; set; }
        public string RecordingStartTime { get; set; }
        public int CurrentDuration { get; set; }
        public string FileFormat { get; set; }
        public string RecorderState { get; set; }
        public string Option { get; set; }

        public bool IsRecording => !string.IsNullOrWhiteSpace(RecordingStartTime);
    }
}
