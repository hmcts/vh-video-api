using System;
using VideoApi.Domain.Enums;

namespace VideoApi.Contract.Requests
{
    public class ParticipantRequest
    {
        public Guid ParticipantRefId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserRole UserRole { get; set; }
        public string CaseTypeGroup { get; set; }
        public string Representee { get; set; }
    }
}
