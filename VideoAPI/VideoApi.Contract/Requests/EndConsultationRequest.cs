using System;

namespace VideoApi.Contract.Requests
{
    public class EndConsultationRequest
    {
        /// <summary>
        /// The conference UUID
        /// </summary>
        public Guid ConferenceId { get; set; }

        /// <summary>
        /// The room Id
        /// </summary>
        public long RoomId { get; set; }
    }
}
