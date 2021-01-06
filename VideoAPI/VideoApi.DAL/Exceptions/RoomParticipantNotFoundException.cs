using System;

namespace VideoApi.DAL.Exceptions
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly

    public class RoomParticipantNotFoundException : Exception
    {
        public RoomParticipantNotFoundException(Guid participantId, long roomId) : base(
          $"Room participant '{participantId}' not found in room '{roomId}")
        {
        }
    }
}
