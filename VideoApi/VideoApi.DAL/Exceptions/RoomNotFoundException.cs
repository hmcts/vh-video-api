using System;
using System.Runtime.Serialization;

namespace VideoApi.DAL.Exceptions
{
    [Serializable]
    public class RoomNotFoundException : VideoDalException
    {
        public RoomNotFoundException(long roomId) : base(
           $"Room '{roomId}' not found ")
        {
        }
        
        public RoomNotFoundException(string roomLabel) : base(
            $"Room '{roomLabel}' not found ")
        {
        }
        public RoomNotFoundException(Guid conferenceId, string roomLabel) : base(
            $"Room '{roomLabel}' not found in conference {conferenceId}")
        {
        }

        protected RoomNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info,context)
        {

        }

    }
}
