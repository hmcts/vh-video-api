using VideoApi.Contract.Responses;
using VideoApi.Domain;

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
                Status = endpoint.State,
                DisplayName = endpoint.DisplayName,
                SipAddress = endpoint.SipAddress,
                DefenceAdvocate = endpoint.DefenceAdvocate
            };
        }
    }
}
