using System;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.DTOs
{
    public class LinkedParticipantDto
    {
        public Guid ParticipantRefId { get; set; }
        public Guid LinkedRefId { get; set; }
        public LinkedParticipantType Type { get; set; }
    }
}
