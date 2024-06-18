using System.Linq;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Extensions;

namespace VideoApi.Mappings
{
    public static class ParticipantResponseMapper
    {
        public static ParticipantResponse Map(ParticipantBase participant, ParticipantRoom participantRoom = null)
        {
            var participantDetailsResponse = new ParticipantResponse();
            participantDetailsResponse.Id = participant.Id;
            participantDetailsResponse.RefId = participant.ParticipantRefId;
            participantDetailsResponse.Username = participant.Username;
            participantDetailsResponse.DisplayName = participant.DisplayName;
            participantDetailsResponse.UserRole = participant.UserRole.MapToContractEnum();
            participantDetailsResponse.CurrentStatus = participant.State.MapToContractEnum();
            participantDetailsResponse.LinkedParticipants = participant.LinkedParticipants.Select(LinkedParticipantToResponseMapper.MapLinkedParticipantsToResponse).ToList();
            participantDetailsResponse.CurrentRoom = RoomToDetailsResponseMapper.MapConsultationRoomToResponse(participant.CurrentConsultationRoom);
            participantDetailsResponse.CurrentInterpreterRoom = RoomToDetailsResponseMapper.MapConsultationRoomToResponse(participantRoom);
            return participantDetailsResponse;
        }
    }
}
