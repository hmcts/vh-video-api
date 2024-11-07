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
            
            // Remove in future iteration, once endpoint BQS added/updated events do not rely on defence advocate contact email
            if (participant is Participant participantCasted)
                participantDetailsResponse.ContactEmail = participantCasted.ContactEmail;
                    
            return participantDetailsResponse;
        }
    }
    
    public static class TelephoneParticipantMapper
    {
        public static TelephoneParticipantResponse Map(TelephoneParticipant telephoneParticipant)
        {
            return new TelephoneParticipantResponse
            {
                Id = telephoneParticipant.Id,
                PhoneNumber = telephoneParticipant.TelephoneNumber,
                Room = telephoneParticipant.CurrentRoom.GetValueOrDefault().MapToContractEnum()
            };
        }
    }
}
