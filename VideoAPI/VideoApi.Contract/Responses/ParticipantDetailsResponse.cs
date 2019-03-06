using System.Collections.Generic;

namespace VideoApi.Contract.Responses
{
    public class ParticipantDetailsResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string HearingRole { get; set; }
        public string CaseTypeGroup { get; set; }
        public List<ParticipantStatusResponse> Statuses { get; set; }
    }
}