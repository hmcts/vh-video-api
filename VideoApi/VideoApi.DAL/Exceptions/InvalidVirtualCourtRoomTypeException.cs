using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Exceptions
{
    public class InvalidVirtualCourtRoomTypeException : VideoDalException
    {
        public InvalidVirtualCourtRoomTypeException(VirtualCourtRoomType roomType, string operationMessage) : base($"VirtualCourtRoomType {roomType} is not valid for this operation: {operationMessage}")
        {
        }
    }
}
