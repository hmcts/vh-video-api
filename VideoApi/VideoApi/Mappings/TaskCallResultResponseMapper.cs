using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Extensions;

namespace VideoApi.Mappings
{
    public static class TaskCallResultResponseMapper
    {
        public static TestCallScoreResponse MapTaskToResponse(TestCallResult testCallScore)
        {
            return new TestCallScoreResponse
            {
                Passed = testCallScore.Passed,
                Score = testCallScore.Score.MapToContractEnum()
            };
        }
    }
}
