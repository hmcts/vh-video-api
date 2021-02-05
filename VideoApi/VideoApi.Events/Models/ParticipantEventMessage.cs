using System;
using VideoApi.Domain.Enums;
using VideoApi.Events.Models.Enums;

namespace VideoApi.Events.Models
{
    public class ParticipantEventMessage : EventMessage
    {
        public Guid ParticipantId { get; set; }
        public ParticipantState ParticipantState { get; set; }
        public override MessageType MessageType => MessageType.Participant;
    }
}
