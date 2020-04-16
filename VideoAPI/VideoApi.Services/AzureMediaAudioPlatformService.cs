using VideoApi.Services.Contracts;

namespace VideoApi.Services
{
    public class AzureMediaAudioPlatformService : IAudioPlatformService
    {
        public string CreateAudioIngestUrl()
        {
            //Call azure media service to create an ingest URL
            return "http://www.thisisadummyURL.net";
        }
    }
}
