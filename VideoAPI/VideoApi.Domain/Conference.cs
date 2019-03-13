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
        }

        public Conference(Guid hearingRefId, string caseType, DateTime scheduledDateTime, string caseNumber) : this()
        {
            HearingRefId = hearingRefId;
            CaseType = caseType;
            ScheduledDateTime = scheduledDateTime;
            CaseNumber = caseNumber;
            
        }

        public Guid HearingRefId { get; protected set; }
        public string CaseType { get; protected set; }
        public DateTime ScheduledDateTime { get; protected set; }
        public string CaseNumber { get; protected set; }
        public virtual VirtualCourt VirtualCourt { get; private set; }
        protected virtual IList<Participant> Participants { get; set; }
        protected virtual IList<ConferenceStatus> ConferenceStatuses { get; set; }
        
        public void UpdateVirtualCourt(string adminUri, string judgeUri, string participantUri, string pexipNode)
        {
            if (VirtualCourt == null)
            {
                VirtualCourt = new VirtualCourt(adminUri, judgeUri, participantUri, pexipNode);
            }
            else
            {
                VirtualCourt.AdminUri = adminUri;
                VirtualCourt.JudgeUri = judgeUri;
                VirtualCourt.ParticipantUri = participantUri;
                VirtualCourt.PexipNode = pexipNode;
            }
            
        }

        public VirtualCourt GetVirtualCourt()
        {
            return VirtualCourt;
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

        private bool DoesParticipantExist(string username)
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