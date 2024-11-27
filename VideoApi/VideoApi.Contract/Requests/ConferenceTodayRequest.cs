using System.Collections.Generic;

namespace VideoApi.Contract.Requests
{
    public class ConferenceTodayRequest
    {
        public IEnumerable<string> HearingVenueNames { get; set; }
    }
}
