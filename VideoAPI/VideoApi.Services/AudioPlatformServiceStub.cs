using System;
using VideoApi.Services.Contracts;

namespace VideoApi.Services
{
    public class AudioPlatformServiceStub : IAudioPlatformService
    {
        public string CreateAudioIngestUrl()
        {
            return $"https://localhost.streaming.mediaServices.windows.net/{Guid.NewGuid()}";
        }
    }
}
