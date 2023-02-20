using System;
using System.Collections.Generic;

namespace VideoApi.Contract.Requests
{
    public class AllocationHearingsToCsoRequest
    {
        public string AllocatedCsoUserName { get; set; }
        public IList<Guid> HearingsRequest { get; set; } = new List<Guid>();
    }
}
