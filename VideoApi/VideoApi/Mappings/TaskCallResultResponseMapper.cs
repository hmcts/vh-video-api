using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace VideoApi.Mappings
{
    public static class TaskCallResultResponseMapper
    {
        public static TestCallScoreResponse MapTaskToResponse(TestCallResult testCallScore)
        {
            return new TestCallScoreResponse
            {
                Passed = testCallScore.Passed,
                Score = testCallScore.Score
            };
        }
    }
}
