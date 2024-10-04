using VideoApi.Contract.Requests;
using VideoApi.Contract.Enums;

namespace VideoApi.Extensions
{
    public static class ConferenceEventRequestExtensions
    {
        public static bool ShouldSkipEventHandler(this ConferenceEventRequest conferenceEvent) =>
            conferenceEvent.EventType is EventType.ConnectingToConference or EventType.ConnectingToEventHub ||
            conferenceEvent.EventType == EventType.Help || conferenceEvent.EventType == EventType.SelectingMedia;
    }
}
