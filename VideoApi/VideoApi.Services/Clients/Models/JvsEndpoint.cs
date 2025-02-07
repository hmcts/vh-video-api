using System.Text;

namespace VideoApi.Services.Clients.Models;

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
}
