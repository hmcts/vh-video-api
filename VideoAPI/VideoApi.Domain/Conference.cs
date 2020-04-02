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
            string caseName, int scheduledDuration, string hearingVenueName, bool audioRecordingRequired, string ingestUrl)
        {
            Id = Guid.NewGuid();
            Participants = new List<Participant>();
            ConferenceStatuses = new List<ConferenceStatus>();
            Tasks = new List<Task>();
            InstantMessageHistory = new List<InstantMessage>();
            MeetingRoom = new MeetingRoom();

            HearingRefId = hearingRefId;
            CaseType = caseType;
            ScheduledDateTime = scheduledDateTime;
            CaseNumber = caseNumber;
            CaseName = caseName;
            ScheduledDuration = scheduledDuration;
            ClosedDateTime = null;
            HearingVenueName = hearingVenueName;
            AudioRecordingRequired = audioRecordingRequired;
            IngestUrl = ingestUrl;
        }

        public Guid HearingRefId { get; private set; }
        public string CaseType { get; private set; }
        public DateTime ScheduledDateTime { get; private set; }
        public DateTime? ClosedDateTime { get; private set; }
        public string CaseNumber { get; private set; }
        public string CaseName { get; private set; }
        protected virtual MeetingRoom MeetingRoom { get; private set; }
        public int ScheduledDuration { get; set; }
        public ConferenceState State { get; private set; }
        public virtual IList<Participant> Participants { get; }
        public virtual IList<ConferenceStatus> ConferenceStatuses { get; }
        public virtual IList<Task> Tasks { get; set; }
        public virtual IList<InstantMessage> InstantMessageHistory { get; }
        public string HearingVenueName { get; private set; }
        public bool AudioRecordingRequired { get; set; }
        public string IngestUrl { get; set; }

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

            if (status == ConferenceState.Closed)
            {
                ClosedDateTime = DateTime.UtcNow;
            }

            State = status;
            ConferenceStatuses.Add(new ConferenceStatus(status));
        }
        
        public void CloseConference()
        {
            ClosedDateTime = DateTime.UtcNow;

            State = ConferenceState.Closed;
            ConferenceStatuses.Add(new ConferenceStatus(ConferenceState.Closed));
        }

        public void AddTask(Guid originId,TaskType taskType, string message)
        {
            var task = new Task(originId, message, taskType);
            Tasks.Add(task);
        }

        public IList<Task> GetTasks()
        {
            return Tasks.OrderByDescending(x => x.Created).ToList();
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

        public void UpdateConferenceDetails(string caseType, string caseNumber, string caseName,
            int scheduledDuration, DateTime scheduledDateTime, string hearingVenueName, bool audioRecordingRequired)
        {
            CaseName = caseName;
            CaseNumber = caseNumber;
            CaseType = caseType;
            ScheduledDateTime = scheduledDateTime;
            ScheduledDuration = scheduledDuration;
            HearingVenueName = hearingVenueName;
            AudioRecordingRequired = audioRecordingRequired;
        }

        public RoomType GetAvailableConsultationRoom()
        {
            if (!Participants.Any())
            {
                throw new DomainRuleException("No Participants", "This conference has no participants");
            }
            
            var consultationRoom1Occupied = GetParticipants().Any(x => x.CurrentRoom == RoomType.ConsultationRoom1);
            if (!consultationRoom1Occupied)
            {
                return RoomType.ConsultationRoom1;
            }
            
            var consultationRoomOccupied = GetParticipants().Any(x => x.CurrentRoom == RoomType.ConsultationRoom2);
            if(!consultationRoomOccupied)
            {
                return RoomType.ConsultationRoom2;
            }
            
            throw new DomainRuleException("Unavailable room", "No consultation rooms available");
        }
        
        public Participant GetJudge()
        {
            return Participants.SingleOrDefault(x => x.IsJudge());
        }

        public Participant GetVideoHearingOfficer()
        {
            return Participants.SingleOrDefault(x => x.IsVideoHearingOfficer());
        }

        public IList<InstantMessage> GetInstantMessageHistory()
        {
            return InstantMessageHistory.OrderByDescending(x => x.TimeStamp).ToList();
        }
        public void AddInstantMessage(string from, string messageText)
        {
            var message = new InstantMessage(from, messageText);
            InstantMessageHistory.Add(message);
        }

        public void ClearInstantMessageHistory()
        {
            InstantMessageHistory.Clear();
        }
    }
}
