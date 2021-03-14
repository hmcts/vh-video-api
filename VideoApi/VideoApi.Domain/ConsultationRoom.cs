using System;
using VideoApi.Domain.Enums;

namespace VideoApi.Domain
{
    public class ConsultationRoom : Room
    {
        public ConsultationRoom(Guid conferenceId, string label, VirtualCourtRoomType type, bool locked) : base(
            conferenceId, label, type, locked)
        {
        }

        public ConsultationRoom(Guid conferenceId, VirtualCourtRoomType type, bool locked) : this(conferenceId, null,
            type, locked)
        {
        }
    }
}
