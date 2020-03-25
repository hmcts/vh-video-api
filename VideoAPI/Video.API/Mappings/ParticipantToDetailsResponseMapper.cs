using System.Collections.Generic;
using System.Linq;
using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace Video.API.Mappings
{
    public static class ParticipantToDetailsResponseMapper
    {
        public static List<ParticipantDetailsResponse> MapParticipantsToResponse(IEnumerable<Participant> participants)
        {
            return participants.Select(participant => new ParticipantDetailsResponse
            {
                Id = participant.Id,
                RefId = participant.ParticipantRefId,
                Name = participant.Name,
                Username = participant.Username,
                DisplayName = participant.DisplayName,
                UserRole = participant.UserRole,
                CaseTypeGroup = participant.CaseTypeGroup,
                Representee = participant.Representee,
                CurrentStatus = participant.State
            })
            .ToList();
        }
    }
}
