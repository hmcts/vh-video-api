using System;
using System.Runtime.Serialization;

namespace VideoApi.DAL.Exceptions
{
    [Serializable]
    public class TaskNotFoundException : Exception
    {
        public TaskNotFoundException(Guid conferenceId, long taskId) : base(
            $"Task '{taskId}' not found in Conference {conferenceId} does not exist")
        {
        }
        
        protected TaskNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
