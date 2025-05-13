using System;
using System.Collections.Generic;
using VideoApi.Services.Clients.Models;

namespace VideoApi.Services.Clients.SupplierStub.Models;

// Json server uses a single model for data and responses
public class Hearing
{
    /// <summary>
    /// The id of the hearing
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("uris")]
    public MeetingUris Uris { get; set; }
    
    [JsonPropertyName("virtual_courtroom_id")]
    public Guid? VirtualCourtroomId { get; set; }
    
    [JsonPropertyName("telephone_conference_id")]
    public string TelephoneConferenceId { get; set; }
    
    [JsonPropertyName("rooms")]
    public List<Room> Rooms { get; set; } = [];
}
