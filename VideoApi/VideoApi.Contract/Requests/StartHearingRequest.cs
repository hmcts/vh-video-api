using System.Collections.Generic;

namespace VideoApi.Contract.Requests
{
    public enum HearingLayout
    {
        Dynamic,
        OnePlus7,
        TwoPlus21
    }
    public class StartHearingRequest
    {
        public StartHearingRequest()
        {
            Layout = HearingLayout.Dynamic;
        }
        public HearingLayout? Layout { get; set; }
        public IEnumerable<string> ParticipantsToForceTransfer { get; set; }
        public bool? MuteGuests { get; set; }
        public string TriggeredByHostId { get; set; }
    }
}
