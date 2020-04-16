using System.Collections.Generic;

namespace VideoApi.Contract.Requests
{
    public class ConferenceForAdminRequest
    {
        public IEnumerable<string> VenueNames { get; set; }
    }
}
