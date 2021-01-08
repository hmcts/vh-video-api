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

        protected RoomNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info,context)
        {

        }

    }
}
