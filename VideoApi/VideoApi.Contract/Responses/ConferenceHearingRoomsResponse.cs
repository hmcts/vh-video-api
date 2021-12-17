using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Responses
{
    public class ConferenceHearingRoomsResponse
    {
        public ConferenceState ConferenceState { get; set; }
        public string TimeStamp { get; set; }
        public string HearingId { get; set; }
    }
}
