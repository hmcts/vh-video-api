using System;
using System.Runtime.Serialization;

namespace VideoApi.DAL.Exceptions
{
    [Serializable]
    public class ParticipantNotFoundException : EntityNotFoundException
    {
        public Guid ParticipantId { get; set; }
        public ParticipantNotFoundException(Guid participantId) : base($"Participant {participantId} does not exist")
        {
            ParticipantId = participantId;
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
