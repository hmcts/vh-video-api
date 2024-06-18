using System;

namespace VideoApi.Contract.Responses
{
    public class AddStaffMemberResponse
    {
        public ParticipantResponse Participant { get; set; }
        public Guid ConferenceId { get; set; }
    }
}
