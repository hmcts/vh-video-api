using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Requests
{
    public class AddMagicLinkParticipantRequest
    {
        public string Name { get; set; }
        public UserRole UserRole { get; set; }
    }
}
