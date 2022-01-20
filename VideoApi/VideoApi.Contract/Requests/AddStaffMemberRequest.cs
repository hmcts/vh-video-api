using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Requests
{
    public class AddStaffMemberRequest
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactEmail { get; set; }
        public UserRole UserRole { get; set; }
        public string HearingRole { get; set; }
    }
}
