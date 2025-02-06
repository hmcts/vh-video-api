using VideoApi.Contract.Enums;
using VideoApi.Domain;
using VideoApi.Services.Clients.Models;
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
                Pin = source.Pin,
                ConferenceRole = (ConferenceRole)source.ConferenceRole
            };
        }

        public static JvsEndpoint MapToEndpoint(EndpointDto source, int index)
        {
            var supplierDisplayName = $"T{100 + index};{source.DisplayName};{source.Id}";
            
            return new JvsEndpoint
            {
                Address = source.SipAddress,
                DisplayName = supplierDisplayName,
                Pin = source.Pin,
                Role = source.ConferenceRole.ToString()
            };
        }
    }
}
