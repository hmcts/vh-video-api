using VideoApi.Contract.Responses;
using VideoApi.Domain.Validations;

namespace Video.API.Mappings
{
    public class TaskCallResultResponseMapper
    {
        public TestCallScoreResponse MapTaskToResponse(TestCallResult testCallScore)
        {
            if (testCallScore == null) return null;
            return new TestCallScoreResponse
            {
                Passed = testCallScore.Passed,
                Score = testCallScore.Score
            };
        }
    }
}