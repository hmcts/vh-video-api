using System.Text;

namespace VideoApi.Services.Clients.Models;

public class CreateConsultationRoomResponse
{
    [JsonPropertyName("room_label")] 
    public string RoomLabel { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class CreateConsultationRoomResponse {\n");
        sb.Append("  RoomLabel: ").Append(RoomLabel).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }
}
