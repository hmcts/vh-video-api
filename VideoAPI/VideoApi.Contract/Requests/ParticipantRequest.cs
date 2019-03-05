using System;

namespace VideoApi.Contract.Requests
{
    public class ParticipantRequest
    {
        public Guid ParticipantRefId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string HearingRole { get; set; }
        public string CaseTypeGroup { get; set; }
    }
}