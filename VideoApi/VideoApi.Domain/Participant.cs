using System;
using System.Collections.Generic;
using VideoApi.Domain.Ddd;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;

namespace VideoApi.Domain
{
    public class Participant : Entity<Guid>
    {
        private Participant()
        {
            Id = Guid.NewGuid();
            ParticipantStatuses = new List<ParticipantStatus>();
        }

        public Participant(Guid participantRefId, string name, string firstName, string lastName, string displayName,
            string username, UserRole userRole, string hearingRole, string caseTypeGroup, string contactEmail,
            string contactTelephone) : this()
        {
            ParticipantRefId = participantRefId;
            DisplayName = displayName;
            Username = username;
            UserRole = userRole;
            HearingRole = hearingRole;
            CaseTypeGroup = caseTypeGroup;
            Name = name;
            FirstName = firstName;
            LastName = lastName;
            ContactEmail = contactEmail;
            ContactTelephone = contactTelephone;
        }

        public Guid ParticipantRefId { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactTelephone { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public UserRole UserRole { get; set; }
        public string HearingRole { get; }
        public string CaseTypeGroup { get; set; }
        public string Representee { get; set; }
        public RoomType? CurrentRoom { get;  set; }
        public long? CurrentVirtualRoomId { get; set; }
        public virtual Room CurrentVirtualRoom { get; set; }
        public long? TestCallResultId { get; set; }
        public virtual TestCallResult TestCallResult { get; private set; }
        protected virtual IList<ParticipantStatus> ParticipantStatuses { get; set; }
        public ParticipantState State { get; set; }
        
        public IList<ParticipantStatus> GetParticipantStatuses()
        {
            return ParticipantStatuses;
        }

        public ParticipantStatus GetCurrentStatus()
        {
            return new ParticipantStatus(State);
        }

        public void UpdateParticipantStatus(ParticipantState status)
        {
            State = status;
            ParticipantStatuses.Add(new ParticipantStatus(status));
        }

        public void UpdateTestCallResult(bool passed, TestScore score)
        {
            TestCallResult = new TestCallResult(passed, score);
        }

        public RoomType GetCurrentRoom()
        {
            return CurrentRoom ?? throw new DomainRuleException(nameof(CurrentRoom), "Participant is not in a room");
        }

        public void UpdateCurrentRoom(RoomType? currentRoom)
        {
            CurrentRoom = currentRoom;
        }

        public void UpdateCurrentVirtualRoom(Room room)
        {
            CurrentVirtualRoom = room;
        }

        public bool IsJudge()
        {
            return UserRole == UserRole.Judge;
        }
        public bool IsVideoHearingOfficer()
        {
            return UserRole == UserRole.VideoHearingsOfficer;
        }
    }
}
