using VideoApi.Domain;

namespace VideoApi.Mappings
{
    public static class ParticipantMapper
    {
        public static Participant MapParticipant(ParticipantBase participantBase)
        {
            return new Participant()
            {
                ParticipantRefId = participantBase.Id,
                DisplayName = participantBase.DisplayName,
                Username = participantBase.Username,
                UserRole = participantBase.UserRole,
                State = participantBase.State,
            };
        }
    }
}
