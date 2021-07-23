using System.Linq;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Extensions;

namespace VideoApi.Mappings
{
    public static class ParticipantToDetailsResponseMapper
    {
        public static ParticipantDetailsResponse MapParticipantToResponse(ParticipantBase participant,
            ParticipantRoom participantRoom = null)
        {
            var participantRoomMapped = participantRoom == null
                ? null
                : RoomToDetailsResponseMapper.MapConsultationRoomToResponse(participantRoom);

            var participantDetailsResponse  = new ParticipantDetailsResponse
            {
                Id = participant.Id,
                RefId = participant.ParticipantRefId,
                Name = participant.Name,
                Username = participant.Username,
                DisplayName = participant.DisplayName,
                UserRole = participant.UserRole.MapToContractEnum(),
                CurrentStatus = participant.State.MapToContractEnum(),
                LinkedParticipants =
                    participant.LinkedParticipants
                        .Select(LinkedParticipantToResponseMapper.MapLinkedParticipantsToResponse).ToList(),
                CurrentRoom =
                    RoomToDetailsResponseMapper.MapConsultationRoomToResponse(participant.CurrentConsultationRoom),
                CurrentInterpreterRoom = participantRoomMapped
            };

            if (participant is Participant participantCasted)
            {
                participantDetailsResponse.FirstName = participantCasted.FirstName;
                participantDetailsResponse.LastName = participantCasted.LastName;
                participantDetailsResponse.HearingRole = participantCasted.HearingRole;
                participantDetailsResponse.CaseTypeGroup = participantCasted.CaseTypeGroup;
                participantDetailsResponse.Representee = participantCasted.Representee;
                participantDetailsResponse.ContactEmail = participantCasted.ContactEmail;
                participantDetailsResponse.ContactTelephone = participantCasted.ContactTelephone;
            }

            return participantDetailsResponse;
        }
    }
}
