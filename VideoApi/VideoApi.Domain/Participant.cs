using System;
using System.Linq;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;

namespace VideoApi.Domain
{
    public class Participant : ParticipantBase
    {
        public Participant()
        {
            Id = Guid.NewGuid();
        }

        public Participant(Guid participantRefId, string name, string firstName, string lastName, string displayName,
            string username, UserRole userRole, string hearingRole, string caseTypeGroup, string contactEmail,
            string contactTelephone, Guid? id = null) : this()
        {
            if(id != null && id != Guid.Empty)
            {
                Id = (Guid)id;
            }
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
        
        public Participant(string name, string firstName, string lastName, string displayName,
            string username, UserRole userRole, string hearingRole, string contactEmail, Guid? id = null) : this()
        {
            if(id != null && id != Guid.Empty)
            {
                Id = (Guid)id;
            }
            DisplayName = displayName;
            Username = username;
            UserRole = userRole;
            HearingRole = hearingRole;
            Name = name;
            FirstName = firstName;
            LastName = lastName;
            ContactEmail = contactEmail;
        }

        /// <summary>
        /// This constructor is used for existing participants, e.g. when updating an existing participant
        /// </summary>
        /// <param name="participantRefId"></param>
        /// <param name="contactEmail"></param>
        /// <param name="contactTelephone"></param>
        /// <param name="displayName"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="name"></param>
        /// <param name="username"></param>
        public Participant(Guid participantRefId, string contactEmail, string contactTelephone,
             string displayName, string firstName, string lastName, string name, string username
            ) : this()
        {
            ParticipantRefId = participantRefId;
            ContactEmail = contactEmail;
            ContactTelephone = contactTelephone;
            DisplayName = displayName;
            FirstName = firstName;
            LastName = lastName;
            Name = name;
            Username = username;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactTelephone { get; set; }
        public string CaseTypeGroup { get; set; }
        public string Representee { get; set; }

        public bool IsJudge()
        {
            return UserRole == UserRole.Judge;
        }

        public bool IsStaffMember()
        {
            return UserRole == UserRole.StaffMember;
        }

        public bool IsVideoHearingOfficer()
        {
            return UserRole == UserRole.VideoHearingsOfficer;
        }
        
        public bool IsAWitness()
        {
            return HearingRole.Equals("Witness", StringComparison.CurrentCultureIgnoreCase);
        }
        
        public override void AddLink(Guid linkedId, LinkedParticipantType linkType)
        {
            var existingLink = LinkedParticipants.SingleOrDefault(x => x.LinkedId == linkedId && x.Type == linkType);
            if (existingLink == null)
            {
                LinkedParticipants.Add(new LinkedParticipant(Id, linkedId, linkType));
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public override void RemoveLink(LinkedParticipant linkedParticipant)
        {
            var link = LinkedParticipants.SingleOrDefault(
                x => x.LinkedId == linkedParticipant.LinkedId && x.Type == linkedParticipant.Type);
            if (link == null)
            {
                throw new DomainRuleException("LinkedParticipant", "Link does not exist");
            }

            LinkedParticipants.Remove(linkedParticipant);
            UpdatedAt = DateTime.UtcNow;
        }
        
        public override void RemoveAllLinks()
        {
            LinkedParticipants.Clear();
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
