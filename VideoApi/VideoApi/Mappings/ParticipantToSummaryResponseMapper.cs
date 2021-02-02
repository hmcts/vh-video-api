using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.Mappings
{
    public static class ParticipantToSummaryResponseMapper
    {
        public static ParticipantSummaryResponse MapParticipantToSummary(Participant participant)
        {
            var participantStatus = participant.GetCurrentStatus() != null
                ? participant.GetCurrentStatus().ParticipantState
                : ParticipantState.None;

            var caseGroup =
                participant.UserRole == UserRole.Individual || participant.UserRole == UserRole.Representative
                    ? participant.CaseTypeGroup
                    : string.Empty;
            
            
            return new ParticipantSummaryResponse
            {
                Id = participant.Id,
                Username = participant.Username,
                DisplayName = participant.DisplayName,
                Status = participantStatus,
                UserRole = participant.UserRole,
                HearingRole = participant.HearingRole,
                Representee = participant.Representee,
                CaseGroup = caseGroup,
                FirstName = participant.FirstName,
                LastName = participant.LastName,
                ContactEmail = participant.ContactEmail,
                ContactTelephone = participant.ContactTelephone
            };
        }
    }
}
