using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace Video.API.Mappings
{
    public class TaskToResponseMapper
    {
        public TaskResponse MapTaskToResponse(Task task)
        {
            return new TaskResponse()
            {
                Id = task.Id,
                Body = task.Body,
                Type = task.Type,
                OriginId = task.OriginId,
                Created = task.Created
            };
        }
    }
}