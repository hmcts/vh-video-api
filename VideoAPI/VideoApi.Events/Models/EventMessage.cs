using System;
using VideoApi.Events.Models.Enums;

namespace VideoApi.Events.Models
{
    public abstract class EventMessage {
        public abstract MessageType MessageType { get; }
        public Guid HearingId { get; set; }
    }
}