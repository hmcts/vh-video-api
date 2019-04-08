namespace VideoApi.Contract.Requests
{
    public enum ConsultationRequestAnswer
    {
        /// <summary>
        /// Default when no answer has been provided
        /// </summary>
        None,
        /// <summary>
        /// Accept a consultation request
        /// </summary>
        Accepted,
        /// <summary>
        /// Reject a consultation request
        /// </summary>
        Rejected
    }
}