using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace Video.API.Mappings
{
    public class ParticipantToSummaryResponseMapper
    {
        public ParticipantSummaryResponse MapParticipantToSummary(Participant participant)
        {
            var participantStatus = participant.GetCurrentStatus() != null
                ? participant.GetCurrentStatus().ParticipantState
                : ParticipantState.None;

            return new ParticipantSummaryResponse
            {
                Username = participant.Username,
                DisplayName = participant.DisplayName,
                Status = participantStatus,
                UserRole = participant.UserRole,
                Representee = participant.Representee
            };
        }
    }
}