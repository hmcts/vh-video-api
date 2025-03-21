using System;
using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Requests
{
    public class ConferenceEventRequest
    {
        /// <summary>
        /// Unique id to represent an event
        /// </summary>
        public string EventId { get; set; }

        /// <summary>
        /// Type of event
        /// </summary>
        public EventType EventType { get; set; }

        /// <summary>
        /// Timestamp when this event has occured (date time in utc).
        /// </summary>
        public DateTime TimeStampUtc { get; set; }

        /// <summary>
        /// Id of virtual court room (uuid).
        /// </summary>
        public string ConferenceId { get; set; }

        /// <summary>
        /// Id of participant (uuid).
        /// </summary>
        public string ParticipantId { get; set; }
        
        /// <summary>
        /// Id of participant room (uuid). This only applies to VMR events
        /// </summary>
        public string ParticipantRoomId { get; set; }

        /// <summary>
        /// Room from where the participant is transferred from.
        /// </summary>
        public string TransferFrom { get; set; }

        /// <summary>
        /// Room to where the participant is transferred to.
        /// </summary>
        public string TransferTo { get; set; }

        /// <summary>
        /// Event reason
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Can be either the phone number or "anonymous"
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Optionally provided conference role. Defaults to Guest
        /// </summary>
        public ConferenceRole? ConferenceRole { get; set; } = Enums.ConferenceRole.Guest;
    }
}
