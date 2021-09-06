using System.Linq;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Extensions;

namespace VideoApi.Mappings
{
    public static class ParticipantToSummaryResponseMapper
    {
        public static ParticipantSummaryResponse MapParticipantToSummary(ParticipantBase participant, ParticipantRoom participantRoom = null)
        {
            var interpreterRoomMapped = participantRoom == null
                ? null
                : RoomToDetailsResponseMapper.MapConsultationRoomToResponse(participantRoom);
            
            var participantStatus = participant.GetCurrentStatus() != null
                ? participant.GetCurrentStatus().ParticipantState
                : ParticipantState.None;

            var caseGroup = participant is Participant ? ((Participant)participant).CaseTypeGroup : string.Empty;

            var links = participant.LinkedParticipants.Select(LinkedParticipantToResponseMapper.MapLinkedParticipantsToResponse)
                .ToList();

            return new ParticipantSummaryResponse
            {
                Id = participant.Id,
                Username = participant.Username,
                DisplayName = participant.DisplayName,
                Status = participantStatus.MapToContractEnum(),
                UserRole = participant.UserRole.MapToContractEnum(),
                HearingRole = participant.HearingRole,
                Representee = participant is Participant ? ((Participant)participant).Representee : null,
                CaseGroup = caseGroup,
                FirstName = participant is Participant ? ((Participant)participant).FirstName : null,
                LastName = participant is Participant ? ((Participant)participant).LastName : null,
                ContactEmail = participant is Participant ? ((Participant)participant).ContactEmail : null,
                ContactTelephone = participant is Participant ? ((Participant)participant).ContactTelephone : null,
                CurrentRoom = RoomToDetailsResponseMapper.MapConsultationRoomToResponse(participant.CurrentConsultationRoom),
                LinkedParticipants = links,
                CurrentInterpreterRoom = interpreterRoomMapped
            };
        }
    }
}
