using VideoApi.Contract.Requests;
using VideoApi.Services.Kinly;

namespace Video.API.Mappings
{
    public static class HearingLayoutMapper
    {
        public static Layout MapLayoutToVideoHearingLayout(HearingLayout layout)
        {
            switch (layout)
            {
                case HearingLayout.OnePlus7: return Layout.ONE_PLUS_SEVEN;
                case HearingLayout.TwoPlus21: return Layout.TWO_PLUS_TWENTYONE;
                default: return Layout.AUTOMATIC;
            }
        }
    }
}
