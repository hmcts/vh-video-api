using System.Text;

namespace VideoApi.Services.Clients.Models
{

    /// <summary>
    /// 
    /// </summary>
    public class TransferRequest
    {
        /// <summary>
        /// Gets or Sets To
        /// </summary>
        [JsonPropertyName("to")]
        public string To { get; set; }

        /// <summary>
        /// Gets or Sets Role
        /// </summary>
        [JsonPropertyName("role")]
        public string Role { get; set; }

        /// <summary>
        /// Gets or Sets FromRoomInfo
        /// </summary>
        [JsonPropertyName("fromRoomInfo")]
        public RoomInfo FromRoomInfo { get; set; }

        /// <summary>
        /// Gets or Sets ToRoomInfo
        /// </summary>
        [JsonPropertyName("toRoomInfo")]
        public RoomInfo ToRoomInfo { get; set; }

        /// <summary>
        /// Gets or Sets From
        /// </summary>
        [JsonPropertyName("from")]
        public string From { get; set; }

        /// <summary>
        /// Gets or Sets PartId
        /// </summary>
        [JsonPropertyName("part_id")]
        public string PartId { get; set; }


        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class TransferRequest {\n");
            sb.Append("  To: ").Append(To).Append("\n");
            sb.Append("  Role: ").Append(Role).Append("\n");
            sb.Append("  FromRoomInfo: ").Append(FromRoomInfo).Append("\n");
            sb.Append("  ToRoomInfo: ").Append(ToRoomInfo).Append("\n");
            sb.Append("  From: ").Append(From).Append("\n");
            sb.Append("  PartId: ").Append(PartId).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
