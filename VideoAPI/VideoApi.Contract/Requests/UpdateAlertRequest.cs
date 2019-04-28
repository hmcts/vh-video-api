namespace VideoApi.Contract.Requests
{
    public class UpdateAlertRequest
    {
        /// <summary>
        /// The username of the person updating the alert
        /// </summary>
        public string UpdatedBy { get; set; }
    }
}