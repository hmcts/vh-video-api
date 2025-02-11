namespace VideoApi.Services.Clients.Models;

public class DisplayNameRequest
{
    /// <summary>
    /// Gets or Sets ParticipantId
    /// </summary>
    [JsonPropertyName("participant_id")]
    public string ParticipantId { get; set; }

    /// <summary>
    /// Gets or Sets ParticipantName
    /// </summary>
    [JsonPropertyName("participant_name")]
    public string ParticipantName { get; set; }
}
