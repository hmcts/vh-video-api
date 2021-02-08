namespace VideoApi.Contract.Requests
{
    public class AddInstantMessageRequest
    {
        /// <summary>
        /// Username of the sender
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Body of the chat message
        /// </summary>
        public string MessageText { get; set; }

        /// <summary>
        /// Username of the receiver
        /// </summary>
        public string To { get; set; }
    }
}
