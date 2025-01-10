using System.Collections.Generic;
using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Responses;

/// <summary>
/// Detailed information for a conference
/// </summary>
public class ConferenceDetailsResponse : ConferenceCoreResponse
{
    /// <summary>
    /// List of participants in conference
    /// </summary>
    public new List<ParticipantResponse> Participants { get; set; } = new();

    /// <summary>
    /// List of connected telephone participants in conference
    /// </summary>
    public List<TelephoneParticipantResponse> TelephoneParticipants { get; set; } = new();

    /// <summary>
    /// List of endpoints in conference
    /// </summary>
    public List<EndpointResponse> Endpoints { get; set; } = new();
    
    /// <summary>
    /// The supplier meeting room details
    /// </summary>
    public MeetingRoomResponse MeetingRoom { get; set; }
    
    /// <summary>
    /// The option to indicate hearing audio recording
    /// </summary>
    public bool AudioRecordingRequired { get; set; }

    public List<CivilianRoomResponse> CivilianRooms { get; set; } = new();
    
    /// <summary>
    /// Ingest Url Audio Recording to be streamed to
    /// </summary>
    public string IngestUrl { get; set; }
    
    public string TelephoneConferenceId { get; set; }
    public string TelephoneConferenceNumbers { get; set; }
    public Supplier Supplier { get; set; }
}
