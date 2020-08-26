using System;
using VideoApi.Domain.Ddd;
using VideoApi.Domain.Enums;

namespace VideoApi.Domain
{
    public class Event : Entity<long>
    {
        public Event(Guid conferenceId, string externalEventId, EventType eventType, DateTime externalTimestamp,
            RoomType? transferredFrom, RoomType? transferredTo, string reason)
        {
            ExternalEventId = externalEventId;
            EventType = eventType;
            ExternalTimestamp = externalTimestamp;
            TransferredFrom = transferredFrom;
            TransferredTo = transferredTo;
            Reason = reason;
            ConferenceId = conferenceId;
            Timestamp = DateTime.UtcNow;
            EndpointFlag = false;
        }

        public Guid ConferenceId { get; set; }
        public string ExternalEventId { get; set; }
        public EventType EventType { get; set; }
        public DateTime ExternalTimestamp { get; set; }
        public Guid ParticipantId { get; set; }
        public RoomType? TransferredFrom { get; set; }
        public RoomType? TransferredTo { get; set; }
        public string Reason { get; set; }
        public DateTime Timestamp { get; set; }
        public bool EndpointFlag { get; set; }
    }
}
