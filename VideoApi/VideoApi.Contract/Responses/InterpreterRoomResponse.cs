namespace VideoApi.Contract.Responses
{
    public class InterpreterRoomResponse
    {
        /// <summary>
        /// The room label
        /// </summary>
        public string Label { get; set; }
        
        /// <summary>
        /// The Pexip node
        /// </summary>
        public string PexipNode { get; set; }
        
        /// <summary>
        /// The join uri for participants
        /// </summary>
        public string ParticipantJoinUri { get; set; }
    }
}
