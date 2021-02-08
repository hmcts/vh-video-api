using System;
using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Responses
{
    public class TaskResponse
    {
        public long Id { get; set; }
        public Guid OriginId { get; set; }
        public string Body { get; set; }
        public TaskType Type { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string UpdatedBy { get; set; }
    }
}