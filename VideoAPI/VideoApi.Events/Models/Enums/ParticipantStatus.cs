using System.Runtime.Serialization;

namespace VideoApi.Events.Models.Enums
{
    public enum ParticipantStatus
    {
        [EnumMember(Value = "Not Signed In")]
        NotSignedIn,
        [EnumMember(Value = "Not Yet Joined")]
        NotYetJoined,
        Joining,
        [EnumMember(Value = "Unable To Join")]
        UnableToJoin,
        Available,
        [EnumMember(Value = "In Consultation")]
        InConsultation,
        [EnumMember(Value = "In Hearing")]
        InHearing,
        Disconnected,
        Unavailable,
        [EnumMember(Value = "In Waiting Room")]
        InWaitingRoom
    }
}