using VideoApi.Domain;
using VideoApi.Services.Dtos;

namespace VideoApi.Services.Mappers
{
    public static class EndpointMapper
    {
        public static EndpointDto MapToEndpoint(Endpoint source)
        {
            return new EndpointDto
            {
                Id = source.Id,
                SipAddress = source.SipAddress,
                DisplayName = source.DisplayName,
                Pin = source.Pin
            };
        }

        public static Kinly.Endpoint MapToEndpoint(EndpointDto source)
        {
            return new Kinly.Endpoint
            {
                Id = source.Id.ToString(),
                Address = source.SipAddress,
                Display_name = source.DisplayName,
                Pin = source.Pin
            };
        }
    }
}
