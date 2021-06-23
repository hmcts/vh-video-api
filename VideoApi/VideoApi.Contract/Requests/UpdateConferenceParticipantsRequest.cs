using System;
using System.Collections.Generic;

namespace VideoApi.Contract.Requests
{
    public class UpdateConferenceParticipantsRequest
    {
        public IList<UpdateParticipantRequest> ExistingParticipants { get; set; } = new List<UpdateParticipantRequest>();
        public IList<ParticipantRequest> NewParticipants { get; set; } = new List<ParticipantRequest>();
        public IList<Guid> RemovedParticipants { get; set; } = new List<Guid>();
        public IList<LinkedParticipantRequest> LinkedParticipants { get; set; } = new List<LinkedParticipantRequest>();
    }
}
