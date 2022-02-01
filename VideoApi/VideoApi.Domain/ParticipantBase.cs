using System;
using System.Collections.Generic;
using System.Linq;
using VideoApi.Domain.Ddd;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;

namespace VideoApi.Domain
{
    public abstract class ParticipantBase : Entity<Guid>
    {

        public Guid ConferenceId { get; private set; }
        public Guid ParticipantRefId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public UserRole UserRole { get; set; }
        public string HearingRole { get; set; }
        public string Username { get; set; }
        public virtual IList<LinkedParticipant> LinkedParticipants { get; set; } = new List<LinkedParticipant>();
        public ParticipantState State { get; set; }
        public RoomType? CurrentRoom { get; set; }
        public long? CurrentConsultationRoomId { get; set; }
        public virtual ConsultationRoom CurrentConsultationRoom { get; set; }
        protected virtual IList<ParticipantStatus> ParticipantStatuses { get; set; } = new List<ParticipantStatus>();
        public virtual IList<RoomParticipant> RoomParticipants { get; } = new List<RoomParticipant>();
        public long? TestCallResultId { get; set; }
        public virtual TestCallResult TestCallResult { get; private set; }

        public void UpdateTestCallResult(bool passed, TestScore score)
        {
            TestCallResult = new TestCallResult(passed, score);
        }

        public virtual void AddLink(Guid linkedId, LinkedParticipantType linkType) { }
        public virtual void RemoveLink(LinkedParticipant linkedParticipant) { }
        public virtual void RemoveAllLinks() { }

        public ParticipantStatus GetCurrentStatus()
        {
            return new ParticipantStatus(State);
        }

        public IList<ParticipantStatus> GetParticipantStatuses()
        {
            return ParticipantStatuses;
        }

        public void UpdateParticipantStatus(ParticipantState status)
        {
            State = status;
            ParticipantStatuses.Add(new ParticipantStatus(status));
        }

        public string GetCurrentRoom()
        {
            return CurrentConsultationRoom?.Label ?? CurrentRoom?.ToString() ?? throw new DomainRuleException(nameof(CurrentRoom), "Participant is not in a room");
        }

        public ParticipantRoom GetParticipantRoom()
        {
            return RoomParticipants.Select(x => x.Room).OfType<ParticipantRoom>().FirstOrDefault();
        }

        public void UpdateCurrentRoom(RoomType? currentRoom)
        {
            CurrentRoom = currentRoom;
        }

        public void UpdateCurrentConsultationRoom(ConsultationRoom consultationRoom)
        {
            if (consultationRoom?.Id == CurrentConsultationRoomId)
            {
                return;
            }
            CurrentConsultationRoom?.RemoveParticipant(new RoomParticipant(Id));
            CurrentConsultationRoom = consultationRoom;
        }

    }
}
