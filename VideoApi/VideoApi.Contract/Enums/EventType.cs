namespace VideoApi.Contract.Enums
{
    public enum EventType
    {
        None = 0,
        Joined = 1,
        Disconnected = 2,
        Transfer = 3,
        Help = 4,
        Start = 17,
        CountdownFinished = 18,
        Pause = 5,
        Close = 6,
        Leave = 7,
        Consultation = 8,
        MediaPermissionDenied = 10,
        ParticipantJoining = 11,
        SelfTestFailed = 12,
        Suspend = 13,
        VhoCall = 14,
        ParticipantNotSignedIn = 16,
        EndpointJoined = 19,
        EndpointDisconnected = 20,
        EndpointTransfer = 21,
        ConnectingToEventHub = 22,
        SelectingMedia = 23,
        ConnectingToConference = 24,
        RoomParticipantJoined = 25,
        RoomParticipantDisconnected = 26,
        RoomParticipantTransfer = 27,
        RecordingConnectionFailed = 28
    }
}
