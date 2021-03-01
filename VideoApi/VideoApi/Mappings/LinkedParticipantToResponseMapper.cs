using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Extensions;

namespace VideoApi.Mappings
{
    public static class LinkedParticipantToResponseMapper
    {
        public static LinkedParticipantResponse MapLinkedParticipantsToResponse(LinkedParticipant linkedParticipant)
        {
            return new LinkedParticipantResponse()
            {
                Type = linkedParticipant.Type.MapToContractEnum(),
                LinkedId = linkedParticipant.LinkedId
            };
        }
    }
}
