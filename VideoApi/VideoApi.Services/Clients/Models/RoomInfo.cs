using System;
using System.Text;

namespace VideoApi.Services.Clients.Models
{

    /// <summary>
    /// 
    /// </summary>
    public class RoomInfo
    {
        /// <summary>
        /// Gets or Sets Alias
        /// </summary>
        [JsonPropertyName("alias")]
        public string Alias { get; set; }

        /// <summary>
        /// Gets or Sets Pin
        /// </summary>
        [JsonPropertyName("pin")]
        public string Pin { get; set; }

        /// <summary>
        /// Gets or Sets SupplierHearingId
        /// </summary>
        [JsonPropertyName("supplierHearingId")]
        public Guid? SupplierHearingId { get; set; }

        /// <summary>
        /// Gets or Sets RoomType
        /// </summary>
        [JsonPropertyName("roomType")]
        public string RoomType { get; set; }

        /// <summary>
        /// Gets or Sets RoomLabel
        /// </summary>
        [JsonPropertyName("roomLabel")]
        public string RoomLabel { get; set; }


        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class RoomInfo {\n");
            sb.Append("  Alias: ").Append(Alias).Append("\n");
            sb.Append("  Pin: ").Append(Pin).Append("\n");
            sb.Append("  SupplierHearingId: ").Append(SupplierHearingId).Append("\n");
            sb.Append("  RoomType: ").Append(RoomType).Append("\n");
            sb.Append("  RoomLabel: ").Append(RoomLabel).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
