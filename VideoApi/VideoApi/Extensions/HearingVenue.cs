using System.Collections.Generic;
using System.Linq;
using VideoApi.Contract.Consts;
using VideoApi.Domain;

namespace VideoApi.Extensions
{
    public static class HearingVenue
    {
        private static readonly List<string> ScottishHearingVenues = new List<string> { HearingVenueNames.Aberdeen, HearingVenueNames.Dundee, HearingVenueNames.Edinburgh, HearingVenueNames.Glasgow, HearingVenueNames.Inverness, HearingVenueNames.Ayr};

        public static bool IsHearingVenueInScotland(this Conference conference)
        {
            return ScottishHearingVenues.Any(venueName => venueName == conference.HearingVenueName);
        }
    }
}
