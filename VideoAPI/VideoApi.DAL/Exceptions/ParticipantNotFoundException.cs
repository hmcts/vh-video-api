using System;

namespace VideoApi.DAL.Exceptions
{
    public class ParticipantNotFoundException : Exception
    {
        public ParticipantNotFoundException(long participantId) : base($"Participant {participantId} does not exist")
        {
        }
    }
}