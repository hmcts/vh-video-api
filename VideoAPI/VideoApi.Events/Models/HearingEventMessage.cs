using VideoApi.Events.Models.Enums;

namespace VideoApi.Events.Models
{
    public class HearingEventMessage : EventMessage
    {
        public HearingEventStatus HearingEventStatus { get; set; }
        public override MessageType MessageType => MessageType.Hearing;
    }
}