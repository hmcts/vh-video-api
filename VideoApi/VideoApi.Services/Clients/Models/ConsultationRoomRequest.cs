namespace VideoApi.Services.Clients.Models;

public class ConsultationRoomRequest
{
    /// <summary>
    /// Gets or Sets RoomLabelPrefix
    /// </summary>
    [JsonPropertyName("room_label_prefix")]
    public string RoomLabelPrefix { get; set; }
}
