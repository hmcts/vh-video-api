using System;
using System.Collections.Generic;

namespace VideoApi.Contract.Requests
{
    public class AnonymiseQuickLinkParticipantWithHearingIdsRequest
    {
        public List<Guid> HearingIds { get; set; }
    }
}
