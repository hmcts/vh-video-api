using System;
using System.Collections.Generic;

namespace VideoApi.Services.Clients.Models;

public class BookHearingRequest
{
    /// <summary>
    /// Gets or Sets RecordingEnabled
    /// </summary>
    [JsonPropertyName("recording_enabled")]
    public bool? RecordingEnabled { get; set; }

    /// <summary>
    /// Gets or Sets StreamingEnabled
    /// </summary>
    [JsonPropertyName("streaming_enabled")]
    public bool? StreamingEnabled { get; set; }

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
    /// Gets or Sets VirtualCourtroomId
    /// </summary>
    [JsonPropertyName("virtual_courtroom_id")]
    public Guid? VirtualCourtroomId { get; set; }

    /// <summary>
    /// Gets or Sets CallbackUri
    /// </summary>
    [JsonPropertyName("callback_uri")]
    public string CallbackUri { get; set; }

    /// <summary>
    /// Gets or Sets RecordingUrl
    /// </summary>
    [JsonPropertyName("recording_url")]
    public string RecordingUrl { get; set; }

    /// <summary>
    /// Gets or Sets StreamingUrl
    /// </summary>
    [JsonPropertyName("streaming_url")]
    public string StreamingUrl { get; set; }

    /// <summary>
    /// Gets or Sets JvsEndpoint
    /// </summary>
    [JsonPropertyName("jvs_endpoint")]
    public List<JvsEndpoint> JvsEndpoint { get; set; }

    /// <summary>
    /// Gets or Sets TelephoneConferenceId
    /// </summary>
    [JsonPropertyName("telephone_conference_id")]
    public string TelephoneConferenceId { get; set; }
}
