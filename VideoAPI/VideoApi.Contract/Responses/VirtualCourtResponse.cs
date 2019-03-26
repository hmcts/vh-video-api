namespace VideoApi.Contract.Responses
{
    /// <summary>
    /// Represents the virtual court information
    /// </summary>
    public class VirtualCourtResponse
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
    }
}