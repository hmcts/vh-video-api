using System;

namespace VideoApi.Common.Configuration
{
    public class AudioRecordingTestIdConfiguration
    {
        public Guid NonExistent { get; set; } = Guid.Parse("e72233b4-e33f-4e2f-bc95-43eabbfaec2a");
        public Guid Existing { get; set; } = Guid.Parse("d27d3b2f-5d23-4bea-85f6-cd4e5cc64918");
    }
}
