using System.Collections.Generic;

namespace VideoApi.Contract.Requests
{
    public class AllocationHearingsToCsoRequest
    {
        public string AllocatedCsoUserName { get; set; }
        public IList<HearingDetail> Hearings { get; set; } = new List<HearingDetail>();
    }

    public abstract class HearingDetail
    {
        public string Time { get; set; }
        public string Judge { get; set; }
        public string CaseName { get; set; }
    }
}
