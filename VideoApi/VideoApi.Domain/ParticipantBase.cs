using System;
using System.Collections.Generic;
using System.Linq;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;

namespace VideoApi.Domain
{
    public abstract class ParticipantBase : TrackableEntity<Guid>
    {
        public Guid ConferenceId { get; set; }
        public Guid ParticipantRefId { get; set; }
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
        public Guid? EndpointId { get; set; } = null;
        public virtual Endpoint Endpoint { get; set; }
        
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
            UpdatedAt = DateTime.UtcNow;
        }
        
        public void UpdateUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new DomainRuleException(nameof(username), "Username is required");
            }
            
            Username = username;
            UpdatedAt = DateTime.UtcNow;
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
            UpdatedAt = DateTime.UtcNow;
        }
        
        public void UpdateCurrentConsultationRoom(ConsultationRoom consultationRoom)
        {
            if (consultationRoom?.Id == CurrentConsultationRoomId)
            {
                return;
            }
            CurrentConsultationRoom?.RemoveParticipant(new RoomParticipant(Id));
            CurrentConsultationRoom = consultationRoom;
            UpdatedAt = DateTime.UtcNow;
        }
        
        public bool CanAutoTransferToHearingRoom()
        {
            return UserRole != UserRole.QuickLinkParticipant && UserRole != UserRole.QuickLinkObserver &&
                   HearingRole != "Witness" && HearingRole != "Expert" && UserRole != UserRole.StaffMember && UserRole != UserRole.Judge;
        }
        
        public bool IsHost()
        {
            return UserRole is UserRole.Judge or UserRole.StaffMember;
        }
    }
}
