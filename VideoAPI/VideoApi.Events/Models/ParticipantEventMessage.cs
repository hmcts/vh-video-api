using System;
using VideoApi.Events.Models.Enums;

namespace VideoApi.Events.Models
{
    public class ParticipantEventMessage : EventMessage
    {
        public Guid ParticipantId { get; set; }
        public ParticipantEventStatus ParticipantEventStatus { get; set; }
        public override MessageType MessageType => MessageType.Participant;
    }
}