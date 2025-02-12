using System.Collections.Generic;

namespace VideoApi.Services.Clients.Models;

/// <summary>
/// 
/// </summary>
public class UpdateHearingRequest
{
    /// <summary>
    /// Gets or Sets RecordingEnabled
    /// </summary>
    [JsonPropertyName("recording_enabled")]
    public bool RecordingEnabled { get; set; } = false;

    /// <summary>
    /// Gets or Sets StreamingEnabled
    /// </summary>
    [JsonPropertyName("streaming_enabled")]
    public bool StreamingEnabled { get; set; } = false;

    /// <summary>
    /// Gets or Sets RoomType
    /// </summary>
    [JsonPropertyName("room_type")]
    public string RoomType { get; set; }

    /// <summary>
    /// Gets or Sets AudioPlaybackLanguage
    /// </summary>
    [JsonPropertyName("audio_playback_language")]
    public string AudioPlaybackLanguage { get; set; }

    /// <summary>
    /// Gets or Sets JvsEndpoint
    /// </summary>
    [JsonPropertyName("jvs_endpoint")]
    public List<JvsEndpoint> JvsEndpoint { get; set; }
}
