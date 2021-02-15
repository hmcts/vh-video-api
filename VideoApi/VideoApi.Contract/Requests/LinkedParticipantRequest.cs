using System;
using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Requests
{
    public class LinkedParticipantRequest
    {
        public Guid ParticipantRefId { get; set; }
        public Guid LinkedRefId { get; set; }
        public LinkedParticipantType Type { get; set; }
    }
}
