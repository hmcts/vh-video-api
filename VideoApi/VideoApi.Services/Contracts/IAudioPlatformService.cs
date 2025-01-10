namespace VideoApi.Services.Contracts;

public interface IAudioPlatformService
{
    /// <summary>
    /// Builds the URI pointing to the location the wowza application that the supplier will stream the audio to
    /// </summary>
    public string GetAudioIngestUrl(string serviceId, string caseNumber, string hearingId);
}
