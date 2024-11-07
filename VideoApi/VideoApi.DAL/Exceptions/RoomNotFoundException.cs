using System;

namespace VideoApi.DAL.Exceptions
{
    public class RoomNotFoundException : EntityNotFoundException
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
    }
}
