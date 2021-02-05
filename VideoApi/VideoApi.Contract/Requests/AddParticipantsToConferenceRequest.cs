using System.Collections.Generic;

namespace VideoApi.Contract.Requests
{
    public class AddParticipantsToConferenceRequest
    {
        public List<ParticipantRequest> Participants { get; set; }
    }
}
