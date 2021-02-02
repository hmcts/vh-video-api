using System;
using System.Runtime.Serialization;

namespace VideoApi.DAL.Exceptions
{
    [Serializable]
    public class RoomParticipantNotFoundException : Exception
    {
        public RoomParticipantNotFoundException(Guid participantId, long roomId) : base(
          $"Room participant '{participantId}' not found in room '{roomId}")
        {
        }

        protected RoomParticipantNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
        {
        }

    }
}

