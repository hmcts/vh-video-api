using System;
using System.Collections.Generic;
using System.Text;

namespace VideoApi.Services.Clients.Models;

/// <summary>
/// 
/// </summary>
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

    /// <summary>
    /// Gets or Sets ConsultationRooms
    /// </summary>
    [JsonPropertyName("consultation_rooms")]
    public List<string> ConsultationRooms { get; set; }
}
