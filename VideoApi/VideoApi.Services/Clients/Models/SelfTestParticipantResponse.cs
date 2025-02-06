using System;
using System.Text;

namespace VideoApi.Services.Clients.Models
{

    /// <summary>
    /// 
    /// </summary>
    public class SelfTestParticipantResponse
    {
        /// <summary>
        /// Gets or Sets Score
        /// </summary>
        [JsonPropertyName("score")]
        public int Score { get; set; }

        /// <summary>
        /// Gets or Sets Passed
        /// </summary>
        [JsonPropertyName("passed")]
        public bool Passed { get; set; }

        /// <summary>
        /// Gets or Sets UserId
        /// </summary>
        [JsonPropertyName("user_id")]
        public Guid? UserId { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class SelfTestParticipantResponse {\n");
            sb.Append("  Score: ").Append(Score).Append("\n");
            sb.Append("  Passed: ").Append(Passed).Append("\n");
            sb.Append("  UserId: ").Append(UserId).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
