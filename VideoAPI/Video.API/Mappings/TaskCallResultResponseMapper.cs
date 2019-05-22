using VideoApi.Contract.Responses;
using VideoApi.Domain.Validations;

namespace Video.API.Mappings
{
    public class TaskCallResultResponseMapper
    {
        public TestCallScoreResponse MapTaskToResponse(TestCallResult task)
        {
            return new TestCallScoreResponse
            {
                Passed = task.Passed,
                Score = task.Score
            };
        }
    }
}