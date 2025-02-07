using System;
using System.Text;

namespace VideoApi.Services.Clients.Models;

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
}
