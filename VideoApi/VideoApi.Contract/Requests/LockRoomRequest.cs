using System;

namespace VideoApi.Contract.Requests
{
    public class LockRoomRequest
    {
        /// <summary>
        /// The conference UUID
        /// </summary>
        public Guid ConferenceId { get; set; }

        /// <summary>
        /// The label / name of the room to lock/unlock
        /// </summary>
        public string RoomLabel { get; set; }

        /// <summary>
        /// The lock state of the room
        /// </summary>
        public bool Lock { get; set; }
    }
}
