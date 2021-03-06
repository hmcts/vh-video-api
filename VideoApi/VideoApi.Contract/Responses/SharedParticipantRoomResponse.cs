using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Responses
{
    public class SharedParticipantRoomResponse
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
        
        /// <summary>
        /// The type of VMR
        /// </summary>
        public VirtualCourtRoomType RoomType { get; set; }
    }
    
    public class WitnessRoomResponse
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
