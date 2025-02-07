using System.Text;

namespace VideoApi.Services.Clients.Models;

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
}
