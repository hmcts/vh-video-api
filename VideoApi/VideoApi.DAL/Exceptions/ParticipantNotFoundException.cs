using System;

namespace VideoApi.DAL.Exceptions
{
    public class ParticipantNotFoundException : EntityNotFoundException
    {
        public Guid ParticipantId { get; set; }
        public ParticipantNotFoundException(Guid participantId) : base($"Participant {participantId} does not exist")
        {
            ParticipantId = participantId;
        }
    }
    
    public class ParticipantLinkException : VideoDalException
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
