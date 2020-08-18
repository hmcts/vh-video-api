namespace VideoApi.Contract.Requests
{
    public class AddEndpointRequest
    {
        /// <summary>
        /// The endpoint display name
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// The endpoint sip address
        /// </summary>
        public string SipAddress { get; set; }
        
        /// <summary>
        /// The endpoint pin
        /// </summary>
        public string Pin { get; set; }
    }
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
