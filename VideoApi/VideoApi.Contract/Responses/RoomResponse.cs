namespace VideoApi.Contract.Responses
{
    public class RoomResponse
    {
        /// <summary>
        /// The room ID
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// The room label
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Is the room locked
        /// </summary>
        public bool Locked { get; set; }
    }
}
