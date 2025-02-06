using System;
using System.Collections.Generic;
using System.Text;

namespace VideoApi.Services.Clients.Models
{

    /// <summary>
    /// 
    /// </summary>
    public class HeartbeatRequest
    {
        /// <summary>
        /// Gets or Sets HearingId
        /// </summary>
        [JsonPropertyName("hearing_id")]
        public Guid? HearingId { get; set; }

        /// <summary>
        /// Gets or Sets ParticipantId
        /// </summary>
        [JsonPropertyName("participant_id")]
        public Guid? ParticipantId { get; set; }

        /// <summary>
        /// Gets or Sets UniqueId
        /// </summary>
        [JsonPropertyName("unique_id")]
        public string UniqueId { get; set; }

        /// <summary>
        /// Gets or Sets SequenceId
        /// </summary>
        [JsonPropertyName("sequence_id")]
        public string SequenceId { get; set; }

        /// <summary>
        /// Gets or Sets Timestamp
        /// </summary>
        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }

        /// <summary>
        /// Gets or Sets ElapsedTime
        /// </summary>
        [JsonPropertyName("elapsed_time")]
        public string ElapsedTime { get; set; }

        /// <summary>
        /// Gets or Sets SessionId
        /// </summary>
        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or Sets MediaStatistics
        /// </summary>
        [JsonPropertyName("media_statistics")]
        public Dictionary<string, Object> MediaStatistics { get; set; }


        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class HeartbeatRequest {\n");
            sb.Append("  HearingId: ").Append(HearingId).Append("\n");
            sb.Append("  ParticipantId: ").Append(ParticipantId).Append("\n");
            sb.Append("  UniqueId: ").Append(UniqueId).Append("\n");
            sb.Append("  SequenceId: ").Append(SequenceId).Append("\n");
            sb.Append("  Timestamp: ").Append(Timestamp).Append("\n");
            sb.Append("  ElapsedTime: ").Append(ElapsedTime).Append("\n");
            sb.Append("  SessionId: ").Append(SessionId).Append("\n");
            sb.Append("  MediaStatistics: ").Append(MediaStatistics).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

    }
}
