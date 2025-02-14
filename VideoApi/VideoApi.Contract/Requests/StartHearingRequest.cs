using System;
using System.Collections.Generic;

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
        public List<Guid> Hosts { get; set; } = [];
        public List<Guid> HostsForScreening { get; set; } = [];
    }
}
