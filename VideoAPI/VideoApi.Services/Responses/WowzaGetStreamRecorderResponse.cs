namespace VideoApi.Services.Responses
{
    public class WowzaGetStreamRecorderResponse
    {
        public int TimeScale { get; set; }
        public string InstanceName { get; set; }
        public string FileVersionDelegateName { get; set; }
        public string ServerName { get; set; }
        public string RecorderName { get; set; }
        public int CurrentSize { get; set; }
        public string SegmentSchedule { get; set; }
        public bool StartOnKeyFrame { get; set; }
        public string OutputPath { get; set; }
        public string CurrentFile { get; set; }
        public string[] SaveFieldList { get; set; }
        public bool DefaultAudioSearchPosition { get; set; }
        public bool RecordData { get; set; }
        public string ApplicationName { get; set; }
        public bool MoveFirstVideoFrameToZero { get; set; }
        public string RecorderErrorString { get; set; }
        public int SegmentSize { get; set; }
        public bool DefaultRecorder { get; set; }
        public bool SplitOnTcDiscontinuity { get; set; }
        public string Version { get; set; }
        public int SkipKeyFrameUntilAudioTimeout { get; set; }
        public string BaseFile { get; set; }
        public int SegmentDuration { get; set; }
        public string RecordingStartTime { get; set; }
        public string FileTemplate { get; set; }
        public int BackBufferTime { get; set; }
        public string SegmentationType { get; set; }
        public int CurrentDuration { get; set; }
        public string FileFormat { get; set; }
        public string RecorderState { get; set; }
        public string Option { get; set; }
    }
}
