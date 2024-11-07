using System;

namespace VideoApi.DAL.Exceptions
{
    public class TaskNotFoundException : EntityNotFoundException
    {
        public TaskNotFoundException(Guid conferenceId, long taskId) : base(
            $"Task '{taskId}' not found in Conference {conferenceId} does not exist")
        {
        }
    }
}
