using System;

namespace VideoApi.Contract.Requests
{
    public enum HearingLayout
    {
        Dynamic,
        OnePlus7,
        TwoPlus21,
        NineEqual,
        SixteenEqual,
        TwentyFiveEqual
    }
    public class StartHearingRequest
    {
        public StartHearingRequest()
        {
            Layout = HearingLayout.Dynamic;
        }
        public HearingLayout? Layout { get; set; }
        public bool? MuteGuests { get; set; }
        public Guid TriggeredByHostId { get; set; }
    }
}
