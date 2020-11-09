using VideoApi.Domain.Enums;

namespace VideoApi.Contract.Requests
{
    public class AddTaskRequest
    {
        /// <summary>
        /// The alert text.
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// The task type.
        /// </summary>
        public TaskType TaskType { get; set; }
    }
}
