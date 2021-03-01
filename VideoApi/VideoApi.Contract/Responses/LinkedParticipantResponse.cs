using System;
using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Responses
{
    public class LinkedParticipantResponse
    {
        public Guid LinkedId { get; set; }
        public LinkedParticipantType Type { get; set; }
    }
}
