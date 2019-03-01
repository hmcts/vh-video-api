using System;
using System.Collections.Generic;
using System.Linq;
using VideoApi.Domain.Ddd;
using VideoApi.Domain.Enums;

namespace VideoApi.Domain
{
    public class Participant : Entity<long>
    {
        public Participant(Guid participantRefId, string name, string displayName, string username, string hearingRole,
            string caseTypeGroup)
        {
            ParticipantRefId = participantRefId;
            DisplayName = displayName;
            Username = username;
            HearingRole = hearingRole;
            CaseTypeGroup = caseTypeGroup;
            Name = name;
        }

        public Guid ParticipantRefId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string HearingRole { get; set; }
        public string CaseTypeGroup { get; set; }
        protected virtual IList<ParticipantStatus> ParticipantStatuses { get; set; }

        public IList<ParticipantStatus> GetParticipantStatuses()
        {
            return ParticipantStatuses;
        }

        public ParticipantStatus GetCurrentStatus()
        {
            return ParticipantStatuses.OrderByDescending(x => x.TimeStamp).FirstOrDefault();
        }

        public void UpdateParticipantStatus(ParticipantState status)
        {
            ParticipantStatuses.Add(new ParticipantStatus(status));
        }
    }
}