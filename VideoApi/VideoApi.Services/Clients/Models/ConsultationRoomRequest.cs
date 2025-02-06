using System.Text;

namespace VideoApi.Services.Clients.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ConsultationRoomRequest
    {
        /// <summary>
        /// Gets or Sets RoomLabelPrefix
        /// </summary>
        [JsonPropertyName("room_label_prefix")]
        public string RoomLabelPrefix { get; set; }


        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ConsultationRoomRequest {\n");
            sb.Append("  RoomLabelPrefix: ").Append(RoomLabelPrefix).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
