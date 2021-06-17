using System;
using System.Collections.Generic;
using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Responses
{
    public class CivilianRoomResponse
    {
        public long Id { get; set; }
        public string Label { get; set; }
        public List<Guid> Participants { get; set; }
        public RoomStatus Status { get; set; }
    }
}
