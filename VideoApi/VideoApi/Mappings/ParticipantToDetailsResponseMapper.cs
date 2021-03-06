using System.Linq;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Extensions;

namespace VideoApi.Mappings
{
    public static class ParticipantToDetailsResponseMapper
    {
        public static ParticipantDetailsResponse MapParticipantToResponse(Participant participant,
            ParticipantRoom participantRoom = null)
        {
            var participantRoomMapped = participantRoom == null
                ? null
                : RoomToDetailsResponseMapper.MapConsultationRoomToResponse(participantRoom);

            return new ParticipantDetailsResponse
            {
                Id = participant.Id,
                RefId = participant.ParticipantRefId,
                Name = participant.Name,
                FirstName = participant.FirstName,
                LastName = participant.LastName,
                Username = participant.Username,
                DisplayName = participant.DisplayName,
                UserRole = participant.UserRole.MapToContractEnum(),
                HearingRole = participant.HearingRole,
                CaseTypeGroup = participant.CaseTypeGroup,
                Representee = participant.Representee,
                CurrentStatus = participant.State.MapToContractEnum(),
                ContactEmail = participant.ContactEmail,
                ContactTelephone = participant.ContactTelephone,
                LinkedParticipants =
                    participant.LinkedParticipants
                        .Select(LinkedParticipantToResponseMapper.MapLinkedParticipantsToResponse).ToList(),
                CurrentRoom =
                    RoomToDetailsResponseMapper.MapConsultationRoomToResponse(participant.CurrentConsultationRoom),
                CurrentInterpreterRoom = participantRoomMapped
            };
        }
    }
}
