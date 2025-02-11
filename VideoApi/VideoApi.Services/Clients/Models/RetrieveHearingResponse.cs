using System;
using System.Collections.Generic;

namespace VideoApi.Services.Clients.Models;

public class RetrieveHearingResponse
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
        
    [JsonPropertyName("telephone_conference_id")]
    public string TelephoneConferenceId { get; set; }
}
