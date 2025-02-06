using System.Text;

namespace VideoApi.Services.Clients.Models
{

    /// <summary>
    /// 
    /// </summary>
    public class JvsEndpoint
    {
        /// <summary>
        /// Gets or Sets Address
        /// </summary>
        [JsonPropertyName("address")]
        public string Address { get; set; }

        /// <summary>
        /// Gets or Sets Pin
        /// </summary>
        [JsonPropertyName("pin")]
        public string Pin { get; set; }

        /// <summary>
        /// Gets or Sets Role
        /// </summary>
        [JsonPropertyName("role")]
        public string Role { get; set; }

        /// <summary>
        /// Gets or Sets DisplayName
        /// </summary>
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class JvsEndpoint {\n");
            sb.Append("  Address: ").Append(Address).Append("\n");
            sb.Append("  Pin: ").Append(Pin).Append("\n");
            sb.Append("  Role: ").Append(Role).Append("\n");
            sb.Append("  DisplayName: ").Append(DisplayName).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
