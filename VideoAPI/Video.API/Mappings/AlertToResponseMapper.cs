using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace Video.API.Mappings
{
    public class AlertToResponseMapper
    {
        public TaskResponse MapAlertToResponse(Task task)
        {
            return new TaskResponse()
            {
                Id = task.Id,
                Body = task.Body,
                Type = task.Type
            };
        }
    }
}