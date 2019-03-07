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
        public string ConferenceId { get; set; }
        public string ParticipantId { get; set; }
        [EnumDataType(typeof(RoomType))]
        public RoomType? TransferFrom { get; set; }
        [EnumDataType(typeof(RoomType))]
        public RoomType? TransferTo { get; set; }
        public string Reason { get; set; }
    }
}