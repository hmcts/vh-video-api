using System;

namespace VideoApi.DAL.Exceptions
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class AlertNotFoundException : Exception
    {
        public AlertNotFoundException(Guid conferenceId, long alertId) : base(
            $"Task '{alertId}' not found in Conference {conferenceId} does not exist")
        {
        }
    }
}