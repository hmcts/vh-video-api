using System;
using VideoApi.Domain.Ddd;
using VideoApi.Domain.Enums;

namespace VideoApi.Domain
{
    public class Event : Entity<long>
    {
        public Event(string externalEventId, EventType eventType, DateTime externalTimestamp, Guid participantId,
            RoomType? transferredFrom, RoomType? transferredTo, string reason)
        {
            ExternalEventId = externalEventId;
            EventType = eventType;
            ExternalTimestamp = externalTimestamp;
            ParticipantId = participantId;
            TransferredFrom = transferredFrom;
            TransferredTo = transferredTo;
            Reason = reason;
            Timestamp = DateTime.UtcNow;
        }

        public string ExternalEventId { get; set; }
        public EventType EventType { get; set; }
        public DateTime ExternalTimestamp { get; set; }
        public Guid ParticipantId { get; set; }
        public RoomType? TransferredFrom { get; set; }
        public RoomType? TransferredTo { get; set; }
        public string Reason { get; set; }
        public DateTime Timestamp { get; set; }
    }
}