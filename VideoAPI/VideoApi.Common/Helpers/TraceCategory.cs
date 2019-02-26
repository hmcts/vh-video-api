namespace VideoApi.Common.Helpers
{
    public enum TraceCategory
    {
        /// <summary>
        /// The hearing
        /// </summary>
        HearingList,

        /// <summary>
        /// The speed test
        /// </summary>
        SpeedTest,

        /// <summary>
        /// The media test
        /// </summary>
        MediaTest,

        /// <summary>
        /// The waiting area
        /// </summary>
        WaitingArea,

        /// <summary>
        /// The lobby
        /// </summary>
        Lobby,

        /// <summary>
        /// The live hearing
        /// </summary>
        LiveHearing,

        /// <summary>
        /// The conversation
        /// </summary>
        ConversationGroups,

        /// <summary>
        /// The premature leave
        /// </summary>
        PrematureLeave,

        /// <summary>
        /// The signal r
        /// </summary>
        SignalR,

        /// <summary>
        /// The eject participant
        /// </summary>
        EjectParticipant,

        /// <summary>
        /// The skype event
        /// </summary>
        SkypeEvent,

        /// <summary>
        /// The meeting status update
        /// </summary>
        MeetingStatusUpdate,

        /// <summary>
        /// The meeting status update
        /// </summary>
        TokenProvider,

        /// <summary>
        /// API Exception
        /// </summary>
        APIException

    }
}
