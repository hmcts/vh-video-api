using VideoApi.Domain.Enums;

namespace VideoApi.Contract.Requests
{
    public class TestScoreResultRequest
    {
        /// <summary>
        /// The score of the participant, Good, Bad or Okay
        /// </summary>
        public TestScore Score { get; set; }

        /// <summary>
        /// If the participants score constitutes a pass
        /// </summary>
        public bool Passed { get; set; }
    }
}
