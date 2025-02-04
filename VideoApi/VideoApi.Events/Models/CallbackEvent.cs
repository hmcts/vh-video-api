using System;
using System.ComponentModel.DataAnnotations;
using VideoApi.Domain.Enums;

namespace VideoApi.Events.Models
{
    public class CallbackEvent
    {
        public string EventId { get; set; }
        public EventType EventType { get; set; }
        public DateTime TimeStampUtc { get; set; }
        public Guid ConferenceId { get; set; }
        public Guid ParticipantId { get; set; }

        [EnumDataType(typeof(RoomType))] public RoomType? TransferFrom { get; set; }

        [EnumDataType(typeof(RoomType))] public RoomType? TransferTo { get; set; }
        public string TransferredFromRoomLabel { get; set; }
        public string TransferredToRoomLabel { get; set; }

        public string Reason { get; set; }
        public string Phone { get; set; }
        public long? ParticipantRoomId { get; set; }
        public ConferenceRole? ConferenceRole { get; set; }
    }
}
