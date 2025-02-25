using System.Collections.Generic;

namespace VideoApi.Services.Clients.Models;

/// <summary>
/// 
/// </summary>
public class StartHearingRequest
{
    /// <summary>
    /// Gets or Sets Hosts
    /// </summary>
    [JsonPropertyName("hosts")]
    public List<string> Hosts { get; set; } = [];

    /// <summary>
    /// Gets or Sets HearingLayout
    /// </summary>
    [JsonPropertyName("hearing_layout")]
    public Layout HearingLayout { get; set; }

    /// <summary>
    /// Gets or Sets MuteGuests
    /// </summary>
    [JsonPropertyName("mute_guests")]
    public bool? MuteGuests { get; set; }

    /// <summary>
    /// Gets or Sets ForceTransferParticipantIds
    /// </summary>
    [JsonPropertyName("force_transfer_participant_ids")]
    public List<string> ForceTransferParticipantIds { get; set; } = [];

    /// <summary>
    /// Gets or Sets TriggeredByHostId
    /// </summary>
    [JsonPropertyName("triggered_by_host_id")]
    public string TriggeredByHostId { get; set; }

    /// <summary>
    /// Gets or Sets HostsForScreening
    /// </summary>
    [JsonPropertyName("hosts_for_screening")]
    public List<string> HostsForScreening { get; set; }
}

public enum Layout
{

    [System.Runtime.Serialization.EnumMember(Value = @"AUTOMATIC")]
    Automatic = 0,

    [System.Runtime.Serialization.EnumMember(Value = @"SINGLE")]
    Single = 1,

    [System.Runtime.Serialization.EnumMember(Value = @"FOUR_EQUAL")]
    FourEqual = 2,

    [System.Runtime.Serialization.EnumMember(Value = @"ONE_PLUS_SEVEN")]
    OnePlusSeven = 3,

    [System.Runtime.Serialization.EnumMember(Value = @"TWO_PLUS_TWENTYONE")]
    TwoPlusTwentyone = 4,

    [System.Runtime.Serialization.EnumMember(Value = @"NINE_EQUAL")]
    NineEqual = 5,
        
    [System.Runtime.Serialization.EnumMember(Value = @"SIXTEEN_EQUAL")]
    SixteenEqual = 6,
        
    [System.Runtime.Serialization.EnumMember(Value = @"TWENTY_FIVE_EQUAL")]
    TwentyFiveEqual = 7
}
