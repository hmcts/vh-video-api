using System;
using System.Collections.Generic;
using System.Text;

namespace VideoApi.Services.Clients.Models;

/// <summary>
/// 
/// </summary>
public class BookHearingResponse
{
    /// <summary>
    /// Gets or Sets Uris
    /// </summary>
    [JsonPropertyName("uris")]
    public MeetingUris Uris { get; set; }

    /// <summary>
    /// Gets or Sets VirtualCourtroomId
    /// </summary>
    [JsonPropertyName("virtual_courtroom_id")]
    public Guid? VirtualCourtroomId { get; set; }
        
    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class BookHearingResponse {\n");
        sb.Append("  Uris: ").Append(Uris).Append("\n");
        sb.Append("  VirtualCourtroomId: ").Append(VirtualCourtroomId).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }
}

public class MeetingUris
{
    [JsonPropertyName("pexip_node")]
    public string PexipNode { get; set; }
        
    [JsonPropertyName("participant")]
    public string Participant { get; set; }
        
    [JsonPropertyName("hearing_room_uri")]
    public string HearingRoomUri { get; set; }
}
