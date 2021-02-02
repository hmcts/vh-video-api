using VideoApi.Contract.Requests;
using VideoApi.Domain.Enums;

namespace Video.API.Extensions
{
    public static class ConferenceEventRequestExtensions
    {
        public static bool ShouldSkipEventHandler(this ConferenceEventRequest conferenceEvent) =>
            !string.IsNullOrEmpty(conferenceEvent.Phone)
                || conferenceEvent.EventType == EventType.ConnectingToConference
                || conferenceEvent.EventType == EventType.ConnectingToEventHub
                || conferenceEvent.EventType == EventType.CountdownFinished
                || conferenceEvent.EventType == EventType.Help
                || conferenceEvent.EventType == EventType.SelectingMedia;
    }
}
