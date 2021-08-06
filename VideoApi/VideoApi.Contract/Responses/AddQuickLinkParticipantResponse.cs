using System;

namespace VideoApi.Contract.Responses
{
    public class AddQuickLinkParticipantResponse
    {
        public ParticipantDetailsResponse ParticipantDetails { get; set; }
        public string Token { get; set; }
        public Guid ConferenceId { get; set; }
    }
}
