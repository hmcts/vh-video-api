using System;

namespace VideoApi.DAL.Exceptions
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class ConferenceNotFoundException : Exception
    {
        public ConferenceNotFoundException(Guid conferenceId) : base($"Conference {conferenceId} does not exist")
        {
        }
    }
}