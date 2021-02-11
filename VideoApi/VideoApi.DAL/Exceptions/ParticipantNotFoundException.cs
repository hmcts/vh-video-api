using System;

namespace VideoApi.DAL.Exceptions
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class ParticipantNotFoundException : Exception
    {
        public ParticipantNotFoundException(Guid participantId) : base($"Participant {participantId} does not exist")
        {
        }
    }
    
    public class ParticipantLinkException : Exception
    {
        public ParticipantLinkException(Guid participantRefId, Guid linkRefId) : base($"Cannot link participants because one or both cannot be found")
        {
            ParticipantRefId = participantRefId;
            LinkRefId = linkRefId;
        }

        public Guid ParticipantRefId { get; }
        public Guid LinkRefId { get; }
    }
}
