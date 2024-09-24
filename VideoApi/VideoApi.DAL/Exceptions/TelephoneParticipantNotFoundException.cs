using System;
using System.Runtime.Serialization;

namespace VideoApi.DAL.Exceptions;

[Serializable]
public class TelephoneParticipantNotFoundException : EntityNotFoundException
{
    public TelephoneParticipantNotFoundException(Guid participantId) : base($"Telephone participant {participantId} does not exist")
    {
    }
        
    protected TelephoneParticipantNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
