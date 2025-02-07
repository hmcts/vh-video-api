using System.Text;

namespace VideoApi.Services.Clients.Models;

public class CreateConsultationRoomResponse
{
    [JsonPropertyName("room_label")] 
    public string RoomLabel { get; set; }
}
