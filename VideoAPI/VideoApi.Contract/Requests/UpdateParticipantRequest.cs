namespace VideoApi.Contract.Requests
{
    public class UpdateParticipantRequest
    {
        /// <summary>
        ///     Participant Fullname
        /// </summary>
        public string Fullname { get; set; }

        /// <summary>
        ///     Participant FirstName
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     Participant LastName
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        ///     Participant Display Name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        ///     Representee
        /// </summary>
        public string Representee { get; set; }
    }
}
