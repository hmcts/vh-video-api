using VideoApi.Domain.Enums;

namespace VideoApi.Domain.Validations
{
    public class TestCallResult
    {
        public TestScore Score { get; set; }
        public bool Passed { get; set; }
    }
}