namespace VideoApi.Services.Clients.Models;

public class MeetingUris
{
    [JsonPropertyName("pexip_node")]
    public string PexipNode { get; set; }
        
    [JsonPropertyName("participant")]
    public string Participant { get; set; }
        
    [JsonPropertyName("hearing_room_uri")]
    public string HearingRoomUri { get; set; }
    
    [JsonPropertyName("telephone_conference_id")]
    public string TelephoneConferenceId { get; set; }
}
