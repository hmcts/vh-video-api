using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace Video.API.Mappings
{
    public class ParticipantStatusResponseMapper
    {
        public ParticipantStatusResponse MapCurrentParticipantStatusToResponse(Participant participant)
        {
            var currentStatus = participant.GetCurrentStatus();
            if (currentStatus == null) return null;

            return new ParticipantStatusResponse
            {
                ParticipantState = currentStatus.ParticipantState,
                TimeStamp = currentStatus.TimeStamp
            };
        }
    }
}