using System;

namespace VideoApi.DAL.Exceptions
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class RoomNotFoundException : Exception
    {
        public RoomNotFoundException(long roomId) : base(
           $"Room '{roomId}' not found ")
        {
        }

    }
}
