using VideoApi.Domain.Ddd;
using VideoApi.Domain.Enums;

namespace VideoApi.Domain
{
    public class TestCallScore: Entity<long>
    {
        public TestCallScore(TestScore score, Participant participant, bool passed)
        {
            Score = score;
            ParticipantId = participant.Id;
            Passed = passed;
        }

        public TestScore Score { get; set; }
        public long ParticipantId { get; set; }
        public bool Passed { get; set; }
    }
}