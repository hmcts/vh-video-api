using System;
using System.Runtime.Serialization;

namespace VideoApi.DAL.Exceptions
{
    [Serializable]
    public class ParticipantNotFoundException : VideoDalException
    {
        public ParticipantNotFoundException(Guid participantId) : base($"Participant {participantId} does not exist")
        {
        }
        
        protected ParticipantNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info,context)
        {
        }
    }
    
    [Serializable]
    public class ParticipantLinkException : VideoDalException
    {
        public ParticipantLinkException(Guid participantRefId, Guid linkRefId) : base($"Cannot link participants because one or both cannot be found")
        {
            ParticipantRefId = participantRefId;
            LinkRefId = linkRefId;
        }
        
        protected ParticipantLinkException(SerializationInfo info, StreamingContext context)
            : base(info,context)
        {
        }

        public Guid ParticipantRefId { get; }
        public Guid LinkRefId { get; }
    }
}
