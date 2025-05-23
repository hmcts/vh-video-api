using System.Linq;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Extensions;

namespace VideoApi.Mappings
{
    public static class EndpointToResponseMapper
    {
        public static EndpointResponse MapEndpointResponse(Endpoint endpoint)
        {
            return new EndpointResponse
            {
                Id = endpoint.Id,
                Pin = endpoint.Pin,
                Status = endpoint.State.MapToContractEnum(),
                DisplayName = endpoint.DisplayName,
                SipAddress = endpoint.SipAddress,
                CurrentRoom = RoomToDetailsResponseMapper.MapConsultationRoomToResponse(endpoint.CurrentConsultationRoom),
                ConferenceRole = (ConferenceRole)endpoint.ConferenceRole,
                ParticipantsLinked = endpoint.ParticipantsLinked?.Select(x => x.Username).ToList() ?? [],
                DefenceAdvocate = string.IsNullOrWhiteSpace(endpoint.DefenceAdvocate) 
                    ? endpoint.ParticipantsLinked?.FirstOrDefault()?.Username
                    : endpoint.DefenceAdvocate,
            };
        }
    }
}
