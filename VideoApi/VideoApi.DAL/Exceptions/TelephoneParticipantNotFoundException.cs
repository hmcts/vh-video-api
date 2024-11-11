using System;

namespace VideoApi.DAL.Exceptions;

public class TelephoneParticipantNotFoundException : EntityNotFoundException
{
    public TelephoneParticipantNotFoundException(Guid participantId) : base($"Telephone participant {participantId} does not exist")
    {
    }
}
