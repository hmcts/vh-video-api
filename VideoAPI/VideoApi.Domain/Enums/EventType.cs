namespace VideoApi.Domain.Enums
{
    public enum EventType
    {
        None,
        Joined,
        Disconnected,
        Transfer,
        Help,
        Pause,
        Close,
        Leave,
        Consultation,
        JudgeAvailable,
        MediaPermissionDenied,
        SelfTestFailed
    }
}