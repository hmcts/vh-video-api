using System.Collections.Generic;

namespace VideoApi.Contract.Requests
{
    public class AllocationHearingsToCsoRequest
    {
        public string AllocatedCsoUserName { get; set; }
        public IList<HearingDetailRequest> Hearings { get; set; } = new List<HearingDetailRequest>();
    }
}
