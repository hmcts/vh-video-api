namespace VideoApi.Contract.Requests
{
    public class AddMessageRequest
    {
        /// <summary>
        /// Username of the sender
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Body of the chat message
        /// </summary>
        public string MessageText { get; set; }
    }
}
