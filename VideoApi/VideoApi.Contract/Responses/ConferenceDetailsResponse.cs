using System.Collections.Generic;

namespace VideoApi.Contract.Responses;

/// <summary>
/// Detailed information for a conference
/// </summary>
public class ConferenceDetailsResponse : ConferenceCoreResponse
{
    /// <summary>
    /// List of participants in conference
    /// </summary>
    public List<ParticipantResponse> Participants { get; set; }
    
    /// <summary>
    /// List of endpoints in conference
    /// </summary>
    public List<EndpointResponse> Endpoints { get; set; }
    
    /// <summary>
    /// The Kinly meeting room details
    /// </summary>
    public MeetingRoomResponse MeetingRoom { get; set; }
    
    /// <summary>
    /// The option to indicate hearing audio recording
    /// </summary>
    public bool AudioRecordingRequired { get; set; }
    
    public List<CivilianRoomResponse> CivilianRooms { get; set; }
    
    /// <summary>
    /// Ingest Url Audio Recording to be streamed to
    /// </summary>
    public string IngestUrl { get; set; }
    
    public string TelephoneConferenceId { get; set; }
    public string TelephoneConferenceNumbers { get; set; }
}
