using VideoApi.Contract.Requests;
using VideoApi.Services.Clients.Models;

namespace VideoApi.Mappings
{
    public static class HearingLayoutMapper
    {
        public static Layout MapLayoutToVideoHearingLayout(HearingLayout layout)
        {
            switch (layout)
            {
                case HearingLayout.OnePlus7: return Layout.OnePlusSeven;
                case HearingLayout.TwoPlus21: return Layout.TwoPlusTwentyone;
                case HearingLayout.NineEqual: return Layout.NineEqual; // aka 3x3
                case HearingLayout.SixteenEqual: return Layout.SixteenEqual; // aka 4x4
                case HearingLayout.TwentyFiveEqual: return Layout.TwentyFiveEqual; //aka 5x5
                default: return Layout.Automatic;
            }
        }
    }
}
