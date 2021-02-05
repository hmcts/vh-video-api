using VideoApi.Domain.Enums;
using VideoApi.Events.Models.Enums;

namespace VideoApi.Events.Models
{
    public class HearingEventMessage : EventMessage
    {
        public ConferenceState ConferenceStatus { get; set; }
        public override MessageType MessageType => MessageType.Hearing;
    }
}
