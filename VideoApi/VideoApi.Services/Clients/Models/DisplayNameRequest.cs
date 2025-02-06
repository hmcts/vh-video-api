using System.Text;

namespace VideoApi.Services.Clients.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class DisplayNameRequest
    {
        /// <summary>
        /// Gets or Sets ParticipantId
        /// </summary>
        [JsonPropertyName("participant_id")]
        public string ParticipantId { get; set; }

        /// <summary>
        /// Gets or Sets ParticipantName
        /// </summary>
        [JsonPropertyName("participant_name")]
        public string ParticipantName { get; set; }


        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class DisplayNameRequest {\n");
            sb.Append("  ParticipantId: ").Append(ParticipantId).Append("\n");
            sb.Append("  ParticipantName: ").Append(ParticipantName).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
