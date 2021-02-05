using System.Net;

namespace VideoApi.Contract.Responses
{
    public class AudioPlatformServiceResponse
    {
        public AudioPlatformServiceResponse(bool success)
        {
            Success = success;
        }

        public bool Success { get; private set; }
        public string Message { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string IngestUrl { get; set; }
    }
}
