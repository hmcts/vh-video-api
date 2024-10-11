using System;
using VideoApi.Contract.Enums;

namespace VideoApi.Services.Dtos
{
    public class EndpointDto
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string SipAddress { get; set; }
        public string Pin { get; set; }
        public ConferenceRole ConferenceRole { get; set; }
    }
}
