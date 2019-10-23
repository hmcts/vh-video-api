namespace VideoApi.Contract.Requests
{
    public class UpdateSelfTestScoreRequest
    {
        /// <summary>
        /// The score of the participant, Good, Bad or Okay
        /// </summary>
        public Domain.Enums.TestScore Score { get; set; }

        /// <summary>
        /// If the participants score constitutes a pass
        /// </summary>
        public bool Passed { get; set; }
    }
}