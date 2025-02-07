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
}
