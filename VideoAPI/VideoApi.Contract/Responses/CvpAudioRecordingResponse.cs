using System.Collections.Generic;

namespace VideoApi.Contract.Responses
{
    public class CvpAudioRecordingResponse
    {
        public IEnumerable<CvpAudioFile> Results { get; set; }    
    }

    public class CvpAudioFile
    {
        public string FileName { get; set; }
        public string SasTokenUrl { get; set; }
    }
}
