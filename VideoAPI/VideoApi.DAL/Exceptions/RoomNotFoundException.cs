using System;
using System.Runtime.Serialization;

namespace VideoApi.DAL.Exceptions
{
    [Serializable]
    public class RoomNotFoundException : Exception
    {
        public RoomNotFoundException(long roomId) : base(
           $"Room '{roomId}' not found ")
        {
        }
        
        public RoomNotFoundException(string roomLabel) : base(
            $"Room '{roomLabel}' not found ")
        {
        }

        protected RoomNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info,context)
        {

        }

    }
}
