using System;

namespace VideoApi.Contract.Responses
{
    public class AddStaffMemberResponse
    {
        public ParticipantDetailsResponse ParticipantDetails { get; set; }
        public Guid ConferenceId { get; set; }
    }
}
