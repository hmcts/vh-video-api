using System.Collections.Generic;
using System.Text;

namespace VideoApi.Services.Clients.Models
{

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


        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class UpdateHearingRequest {\n");
            sb.Append("  RecordingEnabled: ").Append(RecordingEnabled).Append("\n");
            sb.Append("  StreamingEnabled: ").Append(StreamingEnabled).Append("\n");
            sb.Append("  RoomType: ").Append(RoomType).Append("\n");
            sb.Append("  AudioPlaybackLanguage: ").Append(AudioPlaybackLanguage).Append("\n");
            sb.Append("  JvsEndpoint: ").Append(JvsEndpoint).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
