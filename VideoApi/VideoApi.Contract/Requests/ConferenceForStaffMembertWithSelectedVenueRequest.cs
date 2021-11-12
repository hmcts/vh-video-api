using System.Collections.Generic;

namespace VideoApi.Contract.Requests
{
    public class ConferenceForStaffMembertWithSelectedVenueRequest
    {

        public IEnumerable<string> HearingVenueNames { get; set; }
    }
}
