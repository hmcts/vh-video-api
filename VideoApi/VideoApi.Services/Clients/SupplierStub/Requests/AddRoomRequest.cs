using System.Collections.Generic;
using VideoApi.Services.Clients.SupplierStub.Models;

namespace VideoApi.Services.Clients.SupplierStub.Requests;

public class AddRoomRequest
{
    [JsonPropertyName("rooms")]
    public List<Room> Rooms { get; set; }
}
