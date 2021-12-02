using VideoApi.Domain;

namespace VideoApi.Mappings
{
    public static class ParticipantMapper
    {
        public static Participant MapParticipant(ParticipantBase participantBase)
        {

            var s = new Participant()
            {
                ParticipantRefId = participantBase.Id,
                Name = participantBase.Name,
                DisplayName = participantBase.DisplayName,
                Username = participantBase.Username,
                UserRole = participantBase.UserRole,
                State = participantBase.State,
            };
            return s;

        }
    }
}
