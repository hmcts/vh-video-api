using System.Collections.Generic;
using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Requests
{
    public class UpdateEndpointRequest
    {
        public string DisplayName { get; set; }
        public List<string> ParticipantsLinked { get; set; } = new List<string>();
        public ConferenceRole ConferenceRole { get; set; }
    }
}
