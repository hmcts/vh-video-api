using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Requests
{
    public class AddQuickLinkParticipantRequest
    {
        public string Name { get; set; }
        public UserRole UserRole { get; set; }
    }
}
