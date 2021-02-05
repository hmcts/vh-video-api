using System;
using VideoApi.Domain.Enums;

namespace VideoApi.Events.Exceptions
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class RoomTransferException : Exception
    {
        public RoomTransferException(RoomType roomFrom, RoomType roomTo) : base(
            $"Unable to determine participant status from transferEvent - from: {roomFrom}, to: {roomTo}")
        {
        }
        
        public RoomTransferException(string roomFrom, string roomTo) : base(
            $"Unable to determine participant status from transferEvent - from: {roomFrom}, to: {roomTo}")
        {
        }
    }
}
