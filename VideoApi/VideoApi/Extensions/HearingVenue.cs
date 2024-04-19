using System.Collections.Generic;
using VideoApi.Domain;

namespace VideoApi.Extensions
{
    public static class HearingVenue
    {
        private static readonly List<string> ScottishHearingVenues = new () { 
            "Aberdeen Tribunal Hearing Centre",
            "Atlantic Quay Glasgow",
            "Ayr Social Security and Child Support Tribunal",
            "Dumfries (1)",
            "Dundee Tribunal Hearing Centre - Endeavour House",
            "Dunfermline",
            "Edinburgh Employment Appeal Tribunal",
            "Edinburgh Employment Tribunal",
            "Edinburgh Social Security and Child Support Tribunal",
            "Edinburgh Upper Tribunal (Administrative Appeals Chamber)",
            "Galashiels",
            "Glasgow Tribunals Centre",
            "Hamilton Brandon Gate",
            "Hamilton Social Security and Child Support Tribunal",
            "Inverness Employment Tribunal",
            "Inverness Justice Centre",
            "Inverness Social Security and Child Support Tribunal",
            "Kilmarnock",
            "Kirkcaldy",
            "Kirkwall",
            "Lerwick",
            "Stirling Tribunal Hearing Centre",
            "Stranraer",
            "Wick"
        };

        public static bool IsHearingVenueInScotland(this Conference conference)
        {
            return ScottishHearingVenues.Exists(venueName => venueName == conference.HearingVenueName);
        }
    }
}
