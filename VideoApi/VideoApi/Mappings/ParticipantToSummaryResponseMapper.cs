using System.Linq;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Extensions;

namespace VideoApi.Mappings
{
    public static class ParticipantToSummaryResponseMapper
    {
        public static ParticipantSummaryResponse MapParticipantToSummary(Participant participant, InterpreterRoom interpreterRoom = null)
        {
            var interpreterRoomMapped = interpreterRoom == null
                ? null
                : RoomToDetailsResponseMapper.MapConsultationRoomToResponse(interpreterRoom);
            
            var participantStatus = participant.GetCurrentStatus() != null
                ? participant.GetCurrentStatus().ParticipantState
                : ParticipantState.None;

            var caseGroup = participant.CaseTypeGroup ?? string.Empty;

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
                Representee = participant.Representee,
                CaseGroup = caseGroup,
                FirstName = participant.FirstName,
                LastName = participant.LastName,
                ContactEmail = participant.ContactEmail,
                ContactTelephone = participant.ContactTelephone,
                CurrentRoom = RoomToDetailsResponseMapper.MapConsultationRoomToResponse(participant.CurrentConsultationRoom),
                LinkedParticipants = links,
                CurrentInterpreterRoom = interpreterRoomMapped
            };
        }
    }
}
