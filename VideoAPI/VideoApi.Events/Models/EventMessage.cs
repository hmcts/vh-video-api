using System;

namespace VideoApi.Events.Models
{
    public abstract class EventMessage {
        public abstract MessageType MessageType { get; }
        public Guid HearingId { get; set; }
    }

    public enum MessageType
    {
        Participant,
        Hearing
    }
    
    public class HearingEventMessage : EventMessage
    {
        public string HearingStatus { get; set; }
        public override MessageType MessageType => MessageType.Hearing;
    }
    
    public class ParticipantEventMessage : EventMessage
    {
        public Guid ParticipantId { get; set; }
        public string ParticipantStatus { get; set; }
        public override MessageType MessageType => MessageType.Participant;
    }
}