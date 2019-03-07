using System;
using VideoApi.Events.Models.Enums;

namespace VideoApi.Events.Models
{
    public class ParticipantEventMessage : EventMessage
    {
        public Guid ParticipantId { get; set; }
        public ParticipantStatus ParticipantStatus { get; set; }
        public override MessageType MessageType => MessageType.Participant;
    }
}