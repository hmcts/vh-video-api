using System;

namespace VideoApi.DAL.Exceptions
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class ParticipantNotFoundException : Exception
    {
        public ParticipantNotFoundException(long participantId) : base($"Participant {participantId} does not exist")
        {
        }
    }
}