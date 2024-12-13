using System;
using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Requests
{
    /// <summary>
    /// List of directions a witness is transferred
    /// </summary>
    public enum TransferType
    {
        Call,
        Dismiss
    }
    
    /// <summary>
    /// Transfer a witness in a hearing
    /// </summary>
    public class TransferParticipantRequest
    {
        /// <summary>
        /// Participant Id
        /// </summary>
        public Guid? ParticipantId { get; set; }

        /// <summary>
        /// Direction of transfer in regards to a conference
        /// </summary>
        public TransferType? TransferType { get; set; }

        /// <summary>
        /// The role of the participant
        /// </summary>
        public ConferenceRole ConferenceRole { get; set; } = ConferenceRole.Guest;
    }
}
