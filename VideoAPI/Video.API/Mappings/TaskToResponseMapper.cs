using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace Video.API.Mappings
{
    public static class TaskToResponseMapper
    {
        public static TaskResponse MapTaskToResponse(Task task)
        {
            return new TaskResponse
            {
                Id = task.Id,
                Body = task.Body,
                Type = task.Type,
                Status = task.Status,
                OriginId = task.OriginId,
                Created = task.Created,
                Updated = task.Updated,
                UpdatedBy = task.UpdatedBy
            };
        }
    }
}
