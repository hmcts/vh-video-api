using System.Collections.Generic;
using VideoApi.Services.Clients.SupplierStub.Models;

namespace VideoApi.Services.Clients.SupplierStub.Requests;

public class SupplierStubAddRoomRequest
{
    [JsonPropertyName("rooms")]
    public List<SupplierStubRoomModel> Rooms { get; set; }
}
