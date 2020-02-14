using System.Collections.Generic;
using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace Video.API.Mappings
{
    public class ParticipantToDetailsResponseMapper
    {
        public List<ParticipantDetailsResponse> MapParticipantsToResponse(IEnumerable<Participant> participants)
        {
            var response = new List<ParticipantDetailsResponse>();
            foreach (var participant in participants)
            {
                var paResponse = new ParticipantDetailsResponse
                {
                    Id = participant.Id,
                    RefId = participant.ParticipantRefId,
                    Name = participant.Name,
                    Username = participant.Username,
                    DisplayName = participant.DisplayName,
                    UserRole = participant.UserRole,
                    CaseTypeGroup = participant.CaseTypeGroup,
                    Representee = participant.Representee,
                    CurrentStatus = participant.State,
                    SelfTestScore = new TaskCallResultResponseMapper().MapTaskToResponse(participant.TestCallResult)
                };
                response.Add(paResponse);
            }

            return response;
        }
    }
}
