using System;

namespace VideoApi.Services.Dtos
{
    public class EndpointDto
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string SipAddress { get; set; }
        public string Pin { get; set; }
        public bool HasScreeningRequirement { get; set; }
    }
}
