using System;

namespace VideoApi.DAL.Exceptions
{
    public class RoomParticipantNotFoundException : EntityNotFoundException
    {
        public RoomParticipantNotFoundException(Guid participantId, long roomId) : base(
          $"Room participant '{participantId}' not found in room '{roomId}")
        {
        }
    }
}

