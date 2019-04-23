using System;
using VideoApi.Domain.Ddd;
using VideoApi.Domain.Enums;

namespace VideoApi.Domain
{
    public class Alert : Entity<long>
    { 
        public string Body { get; set; }
        public AlertType Type { get; set; }
        public AlertStatus Status { get; set; }
        public DateTime Created { get; set;}
        public DateTime? Updated { get; set; }
        public string UpdatedBy { get; private set; }
        
        public Alert(string body, AlertType type)
        {
            Body = body;
            Type = type;
            Status = AlertStatus.ToDo;
            Created = DateTime.UtcNow;
        }

        /// <summary>
        /// Moves the alert status to done.
        /// </summary>
        /// <param name="completedBy">The username of the user who completed the task</param>
        public void CompleteTask(string completedBy)
        {
            Status = AlertStatus.Done;
            Updated = DateTime.UtcNow;
            UpdatedBy = completedBy;
        }
    }
}