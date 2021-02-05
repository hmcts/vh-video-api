using System;

namespace VideoApi.Contract.Requests
{
    public class LeaveConsultationRequest
    {
        public Guid ConferenceId { get; set; }
        public Guid ParticipantId { get; set; }
    }
}
