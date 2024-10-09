using VideoApi.Contract.Requests;
using VideoApi.Services.Clients;

namespace VideoApi.Mappings
{
    public static class HearingLayoutMapper
    {
        public static Layout MapLayoutToVideoHearingLayout(HearingLayout layout)
        {
            switch (layout)
            {
                case HearingLayout.OnePlus7: return Layout.ONE_PLUS_SEVEN;
                case HearingLayout.TwoPlus21: return Layout.TWO_PLUS_TWENTYONE;
                case HearingLayout.NineEqual: return Layout.NINE_EQUAL; // aka 3x3
                case HearingLayout.SixteenEqual: return Layout.SIXTEEN_EQUAL; // aka 4x4
                case HearingLayout.TwentyFiveEqual: return Layout.TWENTY_FIVE_EQUAL; //aka 5x5
                default: return Layout.AUTOMATIC;
            }
        }
    }
}
