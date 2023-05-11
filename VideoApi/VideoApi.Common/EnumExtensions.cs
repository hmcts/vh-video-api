using VideoApi.Domain.Enums;

namespace VideoApi.Common
{
    public static class EnumExtensions
    {
        public static bool IsEndpointEvent(this EventType eventType)
        {
            return eventType == EventType.EndpointJoined || eventType == EventType.EndpointDisconnected ||
                   eventType == EventType.EndpointTransfer;
        }
    }
}
