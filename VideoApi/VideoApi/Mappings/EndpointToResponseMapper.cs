using System.Linq;
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
                ConferenceRole = (Contract.Enums.ConferenceRole)endpoint.ConferenceRole,
                LinkedParticipantIds = endpoint.ParticipantsLinked?.Select(x => x.Id).ToList()
            };
        }
    }
}
