using System;
using VideoApi.Domain.Enums;

namespace VideoApi.Contract.Responses
{
    public class TaskResponse
    {
        public long Id { get; set; }
        public string Body { get; set; }
        public TaskType Type { get; set; }
    }
}