using System;

namespace VideoApi.Domain
{
    public class HearingAudioRoom
    {
        public Guid HearingRefId { get; set; }
        public string Label { get; set; }
        public string FileNamePrefix { get; set; }

    }
}
