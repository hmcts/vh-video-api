using System;
using System.Collections.Generic;
using VideoApi.Domain.Ddd;

namespace VideoApi.Domain.Validations
{
    public class CourtRoom : Entity<long>
    {
        public CourtRoom(Guid conferenceId)
        {
            ConferenceId = conferenceId;
            RoomHistory = new List<RoomParticipant>();
            OpenTime = DateTime.UtcNow;
        }

        public Guid ConferenceId { get; set; }
        public DateTime OpenTime { get; private set; }
        public DateTime CloseTime { get; private set; }
        
        protected virtual IList<RoomParticipant> RoomHistory { get; set; }
        
        public void CloseCourtRoom()
        {
            CloseTime = DateTime.UtcNow;
        }

        public void AddParticipant(Participant participant)
        {
            RoomHistory.Add(new RoomParticipant(this, participant));
        }
        
    }
}