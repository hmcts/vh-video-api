using System;

namespace VideoApi.DAL.Exceptions
{
    public class ParticipantNotFoundException : EntityNotFoundException
    {
        public ParticipantNotFoundException(Guid participantId) : base($"Participant {participantId} does not exist")
        {
            ParticipantId = participantId;
        }
        
        public ParticipantNotFoundException(string username) : base($"Participant {username} does not exist")
        {
            Username = username;
        }
        
        public Guid ParticipantId { get; set; }
        public string Username { get; set; }
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
