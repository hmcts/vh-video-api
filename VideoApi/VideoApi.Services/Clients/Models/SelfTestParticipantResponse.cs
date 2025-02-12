using System;

namespace VideoApi.Services.Clients.Models;

/// <summary>
/// 
/// </summary>
public class SelfTestParticipantResponse
{
    /// <summary>
    /// Gets or Sets Score
    /// </summary>
    [JsonPropertyName("score")]
    public int Score { get; set; }

    /// <summary>
    /// Gets or Sets Passed
    /// </summary>
    [JsonPropertyName("passed")]
    public bool Passed { get; set; }

    /// <summary>
    /// Gets or Sets UserId
    /// </summary>
    [JsonPropertyName("user_id")]
    public Guid? UserId { get; set; }
}
