using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Extensions;

namespace VideoApi.Mappings
{
    public static class TaskToResponseMapper
    {
        public static TaskResponse MapTaskToResponse(Task task)
        {
            return new TaskResponse
            {
                Id = task.Id,
                Body = task.Body,
                Type = task.Type.MapToContractEnum(),
                Status = task.Status.MapToContractEnum(),
                OriginId = task.OriginId,
                Created = task.Created,
                Updated = task.Updated,
                UpdatedBy = task.UpdatedBy
            };
        }
    }
}
