using System;
using System.Collections.Generic;
using System.Linq;
using VideoApi.Domain.Ddd;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;

namespace VideoApi.Domain
{
    public class Conference : Entity<Guid>
    {
        public Conference(Guid hearingRefId, string caseType, DateTime scheduledDateTime, string caseNumber,
            string caseName, int scheduledDuration)
        {
            Id = Guid.NewGuid();
            Participants = new List<Participant>();
            ConferenceStatuses = new List<ConferenceStatus>();
            Tasks = new List<Task>();
            MeetingRoom = new MeetingRoom();

            HearingRefId = hearingRefId;
            CaseType = caseType;
            ScheduledDateTime = scheduledDateTime;
            CaseNumber = caseNumber;
            CaseName = caseName;
            ScheduledDuration = scheduledDuration;
        }

        public Guid HearingRefId { get; protected set; }
        public string CaseType { get; protected set; }
        public DateTime ScheduledDateTime { get; protected set; }
        public string CaseNumber { get; protected set; }
        public string CaseName { get; protected set; }
        protected virtual MeetingRoom MeetingRoom { get; private set; }
        public int ScheduledDuration { get; set; }
        public ConferenceState State { get; private set; }
        public virtual IList<Participant> Participants { get; private set; }
        public virtual IList<ConferenceStatus> ConferenceStatuses { get; private set; }
        public virtual IList<Task> Tasks { get; private set; }

        public void UpdateMeetingRoom(string adminUri, string judgeUri, string participantUri, string pexipNode)
        {
            MeetingRoom = new MeetingRoom(adminUri, judgeUri, participantUri, pexipNode);
        }

        public MeetingRoom GetMeetingRoom()
        {
            return MeetingRoom.IsSet() ? MeetingRoom : null;
        }

        public void AddParticipant(Participant participant)
        {
            if (DoesParticipantExist(participant.Username))
            {
                throw new DomainRuleException(nameof(participant), "Participant already exists in conference");
            }

            Participants.Add(participant);
        }

        public void RemoveParticipant(Participant participant)
        {
            if (!DoesParticipantExist(participant.Username))
            {
                throw new DomainRuleException(nameof(participant), "Participant does not exist in conference");
            }

            var existingParticipant = Participants.Single(x => x.Username == participant.Username);
            Participants.Remove(existingParticipant);
        }

        public bool DoesParticipantExist(string username)
        {
            return Participants.Any(x => x.Username == username);
        }

        public IList<Participant> GetParticipants()
        {
            return Participants;
        }

        public void UpdateConferenceStatus(ConferenceState status)
        {
            if (status == ConferenceState.NotStarted)
            {
                throw new DomainRuleException(nameof(status), "Cannot set conference status to 'none'");
            }

            State = status;
            ConferenceStatuses.Add(new ConferenceStatus(status));
        }

        public void AddTask(TaskType taskType, string message)
        {
            var task = new Task(message, taskType);
            Tasks.Add(task);
        }

        public IList<Task> GetTasks()
        {
            return Tasks;
        }

        public IList<ConferenceStatus> GetConferenceStatuses()
        {
            return ConferenceStatuses;
        }

        public ConferenceState GetCurrentStatus()
        {
            return State;
        }

        public bool IsClosed()
        {
            return State == ConferenceState.Closed;
        }
    }
}