using System;
using VideoApi.Domain.Ddd;
using VideoApi.Domain.Enums;

namespace VideoApi.Domain
{
    public class Task : Entity<long>
    {
        /// <summary>
        /// Origin - entity on which this task creation is based on
        /// </summary>
        public Guid OriginId { get; set; }
        public string Body { get; set; }
        public TaskType Type { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime Created { get; set;}
        public DateTime? Updated { get; set; }
        public string UpdatedBy { get; private set; }
        public Guid ConferenceId { get; set; }
        
        public Task(Guid originId, string body, TaskType type)
        {
            OriginId = originId;
            Body = body;
            Type = type;
            Status = TaskStatus.ToDo;
            Created = DateTime.UtcNow;
        }

        /// <summary>
        /// Moves the alert status to done.
        /// </summary>
        /// <param name="completedBy">The username of the user who completed the task</param>
        public void CompleteTask(string completedBy)
        {
            Status = TaskStatus.Done;
            Updated = DateTime.UtcNow;
            UpdatedBy = completedBy;
        }
    }
}
