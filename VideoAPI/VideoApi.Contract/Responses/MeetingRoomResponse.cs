namespace VideoApi.Contract.Responses
{
    /// <summary>
    /// Represents the meeting room
    /// </summary>
    public class MeetingRoomResponse
    {
        /// <summary>
        /// The iFrame uri for the video hearings officer
        /// </summary>
        public string AdminUri { get; set; }

        /// <summary>
        /// The iFrame uri for the judge
        /// </summary>
        public string JudgeUri { get; set; }

        /// <summary>
        /// The meeting uri for participants
        /// </summary>
        public string ParticipantUri { get; set; }

        /// <summary>
        /// The Pexip node to connect to
        /// </summary>
        public string PexipNode { get; set; }
        
        /// <summary>
        /// The pexip node to connect to for self test
        /// </summary>
        public string PexipSelfTestNode { get; set; }

        /// <summary>
        /// Public switched telephone network pin
        /// </summary>
        public string PstnPin { get; set; }
    }
}
