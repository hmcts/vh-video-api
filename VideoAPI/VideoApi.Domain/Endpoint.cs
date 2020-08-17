using System;
using VideoApi.Domain.Ddd;
using VideoApi.Domain.Enums;

namespace VideoApi.Domain
{
    public sealed class Endpoint : Entity<Guid>
    {
        public string DisplayName { get; set; }
        public string SipAddress { get; }
        public string Pin { get; }
        public EndpointState State { get; private set; }

        private Endpoint()
        {
            Id = Guid.NewGuid();
        }

        public Endpoint(string displayName, string sipAddress, string pin): this()
        {
            DisplayName = displayName;
            SipAddress = sipAddress;
            Pin = pin;
            State = EndpointState.NotYetJoined;
        }

        public void UpdateDisplayName(string displayName)
        {
            DisplayName = displayName;
        }

        public void UpdateStatus(EndpointState status)
        {
            State = status;
        }
    }
}
