using System;
using System.Collections.Generic;

namespace VideoApi.Contract.Requests
{
    public class AnonymiseConferenceWithHearingIdsRequest
    {
        public List<Guid> HearingIds { get; set; }
    }
}
