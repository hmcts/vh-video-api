using System;
using System.Collections.Generic;
using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Responses;

/// <summary>
/// Core conference information
/// </summary>
public class ConferenceCoreResponse
{
    /// <summary>
    /// The conference's UUID
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// The UUID of the booking
    /// </summary>
    public Guid HearingId { get; set; }
    
    /// <summary>
    /// The scheduled start time of a conference
    /// </summary>
    public DateTime ScheduledDateTime { get; set; }
    
    /// <summary>
    /// The time a conference started (not resumed)
    /// </summary>
    public DateTime? StartedDateTime { get; set; }
    
    /// <summary>
    /// The time a conference was closed
    /// </summary>
    public DateTime? ClosedDateTime { get; set; }
    
    /// <summary>
    /// The scheduled duration of a conference in minutes
    /// </summary>
    public int ScheduledDuration { get; set; }
    
    /// <summary>
    /// The current conference status
    /// </summary>
    public ConferenceState CurrentStatus { get; set; }
    
    /// <summary>
    /// Is the waiting room still accessible for the conference
    /// </summary>
    public bool IsWaitingRoomOpen { get; set; }

    /// <summary>
    /// List of participants in conference
    /// </summary>
    public List<ParticipantCoreResponse> Participants { get; set; } = new();
    
    /// <summary>
    /// FOR TESTING ONLY: The case name of the conference
    /// </summary>
    public string CaseName { get; set; }

    /// <summary>
    /// The room type of the conference
    /// </summary>
    public ConferenceRoomType ConferenceRoomType { get; set; }

    /// <summary>
    /// The language for the conference used to determine the audio of the countdown language and waiting room message
    /// </summary>
    public AudioPlaybackLanguage AudioPlaybackLanguage { get; set; }
}
