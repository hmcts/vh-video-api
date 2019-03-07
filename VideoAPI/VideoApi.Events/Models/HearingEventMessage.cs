using VideoApi.Events.Models.Enums;

namespace VideoApi.Events.Models
{
    public class HearingEventMessage : EventMessage
    {
        public HearingStatus HearingStatus { get; set; }
        public override MessageType MessageType => MessageType.Hearing;
    }
}