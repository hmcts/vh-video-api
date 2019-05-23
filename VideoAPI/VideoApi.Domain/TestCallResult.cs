using VideoApi.Domain.Ddd;
using VideoApi.Domain.Enums;

namespace VideoApi.Domain
{
    public class TestCallResult : Entity<long>
    {
        public TestScore Score { get; private set; }
        public bool Passed { get; private set; }
        
        public TestCallResult(bool passed, TestScore score)
        {
            Passed = passed;
            Score = score;
        }
    }
}