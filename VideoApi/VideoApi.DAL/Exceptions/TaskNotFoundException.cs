using System;

namespace VideoApi.DAL.Exceptions
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class TaskNotFoundException : Exception
    {
        public TaskNotFoundException(Guid conferenceId, long taskId) : base(
            $"Task '{taskId}' not found in Conference {conferenceId} does not exist")
        {
        }
    }
}
