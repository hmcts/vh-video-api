namespace VideoApi.Services.Clients.Models;

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
    /// Gets or Sets From
    /// </summary>
    [JsonPropertyName("from")]
    public string From { get; set; }

    /// <summary>
    /// Gets or Sets PartId
    /// </summary>
    [JsonPropertyName("part_id")]
    public string PartId { get; set; }
}
