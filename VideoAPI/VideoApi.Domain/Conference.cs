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
        private Conference()
        {
            Id = Guid.NewGuid();
            Participants = new List<Participant>();
            ConferenceStatuses = new List<ConferenceStatus>();
            MeetingRoom = new MeetingRoom();
        }

        public Conference(Guid hearingRefId, string caseType, DateTime scheduledDateTime, string caseNumber,
            string caseName, int scheduledDuration) : this()
        {
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
        public virtual IList<Participant> Participants { get; private set; }
        protected virtual IList<ConferenceStatus> ConferenceStatuses { get; private set; }

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
            ConferenceStatuses.Add(new ConferenceStatus(status));
        }

        public IList<ConferenceStatus> GetConferenceStatuses()
        {
            return ConferenceStatuses;
        }

        public ConferenceStatus GetCurrentStatus()
        {
            return ConferenceStatuses.OrderByDescending(x => x.TimeStamp).FirstOrDefault();
        }
    }
}