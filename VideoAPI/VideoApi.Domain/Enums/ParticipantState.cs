namespace VideoApi.Domain.Enums
{
    public enum ParticipantState
    {
        NotSignedIn,
        UnableToJoin,
        Joining,
        InWaitingRoom,
        InHearing,
        InConsultation,
        Disconnected
    }
}