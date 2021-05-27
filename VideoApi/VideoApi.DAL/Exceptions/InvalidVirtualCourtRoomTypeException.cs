using System;
using System.Runtime.Serialization;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Exceptions
{
    [Serializable]
    public class InvalidVirtualCourtRoomTypeException : Exception
    {
        public InvalidVirtualCourtRoomTypeException(VirtualCourtRoomType roomType, string operationMessage) : base($"VirtualCourtRoomType {roomType} is not valid for this operation: {operationMessage}")
        {
        }
        
        protected InvalidVirtualCourtRoomTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
