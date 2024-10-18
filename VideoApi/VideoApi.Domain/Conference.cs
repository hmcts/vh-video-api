using System;
using System.Collections.Generic;
using System.Linq;
using RandomStringCreator;
using VideoApi.Domain.Ddd;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;

namespace VideoApi.Domain
{
    public class Conference : Entity<Guid>
    {
        public Conference(Guid hearingRefId, string caseType, DateTime scheduledDateTime, string caseNumber,
            string caseName, int scheduledDuration, string hearingVenueName, bool audioRecordingRequired, string ingestUrl,
            Supplier supplier = Supplier.Kinly, ConferenceRoomType conferenceRoomType = ConferenceRoomType.VMR, AudioPlaybackLanguage audioPlaybackLanguage = AudioPlaybackLanguage.EnglishAndWelsh)
        {
            Id = Guid.NewGuid();
            Participants = new List<ParticipantBase>();
            ConferenceStatuses = new List<ConferenceStatus>();
            InstantMessageHistory = new List<InstantMessage>();
            MeetingRoom = new MeetingRoom();
            Endpoints = new List<Endpoint>();
            TelephoneParticipants = new List<TelephoneParticipant>();
            _rooms = new List<Room>();

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
            CreatedDateTime = DateTime.UtcNow;
            UpdatedAt = CreatedDateTime;
            Supplier = supplier;
            ConferenceRoomType = conferenceRoomType;
            AudioPlaybackLanguage = audioPlaybackLanguage;
        }

        public Guid HearingRefId { get; private set; }
        public string CaseType { get; private set; }
        public DateTime ScheduledDateTime { get; private set; }
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ClosedDateTime { get; set; }
        public string CaseNumber { get; private set; }
        public string CaseName { get; private set; }
        public MeetingRoom MeetingRoom { get; private set; }
        public int ScheduledDuration { get; set; }
        public ConferenceState State { get; set; }
        public virtual IList<ParticipantBase> Participants { get; }
        public virtual IList<Endpoint> Endpoints { get; }
        protected virtual IList<TelephoneParticipant> TelephoneParticipants { get; }
        public virtual IList<ConferenceStatus> ConferenceStatuses { get; }
        public virtual IList<InstantMessage> InstantMessageHistory { get; }
        public string HearingVenueName { get; private set; }
        public bool AudioRecordingRequired { get; set; }
        public string IngestUrl { get; set; }
        public DateTime? CreatedDateTime { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private readonly List<Room> _rooms;
        public IReadOnlyCollection<Room> Rooms => _rooms.AsReadOnly();
        public Supplier Supplier { get; private set; }
        public ConferenceRoomType ConferenceRoomType { get; private set; }
        public AudioPlaybackLanguage AudioPlaybackLanguage { get; private set; }
        
        public void UpdateMeetingRoom(string adminUri, string judgeUri, string participantUri, string pexipNode,
            string telephoneConferenceId)
        {
            MeetingRoom = new MeetingRoom(adminUri, judgeUri, participantUri, pexipNode, telephoneConferenceId);
        }

        public MeetingRoom GetMeetingRoom()
        {
            return MeetingRoom.IsSet() ? MeetingRoom : null;
        }

        public void AddParticipant(ParticipantBase participant)
        {
            if (DoesParticipantExist(participant.Username))
            {
                throw new DomainRuleException(nameof(participant), $"Participant already exists in conference {participant.Username}");
            }

            Participants.Add(participant);
        }

        public void RemoveParticipant(ParticipantBase participant)
        {
            if (!DoesParticipantExist(participant.Username))
            {
                throw new DomainRuleException(nameof(participant), $"Participant does not exist in conference {participant.Username}");
            }

            var existingParticipant = Participants.Single(x => x.Username == participant.Username);
            Participants.Remove(existingParticipant);
        }

        public bool DoesParticipantExist(string username)
        {
            return Participants.Any(x => x.Username == username);
        }

        public IList<ParticipantBase> GetParticipants()
        {
            return Participants;
        }
        
        public void AddEndpoint(Endpoint endpoint)
        {
            if (DoesEndpointExist(endpoint.SipAddress))
            {
                throw new DomainRuleException(nameof(endpoint), "Endpoint already exists in conference");
            }

            Endpoints.Add(endpoint);
        }

        public void RemoveEndpoint(Endpoint endpoint)
        {
            if (!DoesEndpointExist(endpoint.SipAddress))
            {
                throw new DomainRuleException(nameof(endpoint), "Endpoint does not exist in conference");
            }

            var existingEndpoint = Endpoints.Single(x => x.SipAddress == endpoint.SipAddress);
            Endpoints.Remove(existingEndpoint);
        }

        private bool DoesEndpointExist(string sipAddress)
        {
            return Endpoints.Any(x => x.SipAddress == sipAddress);
        }

        public IList<Endpoint> GetEndpoints()
        {
            return Endpoints;
        }

        public void UpdateConferenceStatus(ConferenceState status)
        {
            if (status == ConferenceState.NotStarted)
            {
                throw new DomainRuleException(nameof(status), "Cannot set conference status to 'Not Started'");
            }

            if (status == ConferenceState.InSession && !ActualStartTime.HasValue)
            {
                ActualStartTime = DateTime.UtcNow;
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

        public bool IsConferenceAccessible()
        {
            if (State != ConferenceState.Closed)
            {
                return true;
            }

            if (!ClosedDateTime.HasValue)
            {
                throw new DomainRuleException(nameof(ClosedDateTime), "A closed conference must have a closed time");
            }
            // After a conference is closed, VH Officers can still administer conferences until this period of time
            const int postClosedVisibilityTime = 120;
            var endTime = ClosedDateTime.Value.AddMinutes(postClosedVisibilityTime);
            return DateTime.UtcNow < endTime;
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

        public ParticipantBase GetJudge()
        {
            return Participants.SingleOrDefault(x => x is Participant && ((Participant)x).IsJudge());
        }

        public ParticipantBase GetVideoHearingOfficer()
        {
            return Participants.SingleOrDefault(x => x is Participant && ((Participant)x).IsVideoHearingOfficer());
        }

        public IList<InstantMessage> GetInstantMessageHistory()
        {
            return InstantMessageHistory.OrderByDescending(x => x.TimeStamp).ToList();
        }
        public IList<InstantMessage> GetInstantMessageHistoryFor(string participantName)
        {
            return InstantMessageHistory.Where(x => x.From.ToUpper() == participantName.ToUpper() || x.To.ToUpper() == participantName.ToUpper())
                .OrderByDescending(x => x.TimeStamp).ToList();
        }
        public void AddInstantMessage(string from, string messageText, string to)
        {
            var message = new InstantMessage(from, messageText, to);
            InstantMessageHistory.Add(message);
        }

        public void ClearInstantMessageHistory()
        {
            InstantMessageHistory.Clear();
        }

        public void AddLinkedParticipants(Guid primaryParticipantRefId, Guid secondaryParticipantRefId, LinkedParticipantType linkedParticipantType)
        {
            var primaryParticipant =
                Participants.Single(x => x.ParticipantRefId == primaryParticipantRefId);

            var secondaryParticipant =
                Participants.Single(x => x.ParticipantRefId == secondaryParticipantRefId);

            if (primaryParticipant is Participant && secondaryParticipant is Participant)
            {

                ((Participant)primaryParticipant).AddLink(secondaryParticipant.Id, linkedParticipantType);
                ((Participant)secondaryParticipant).AddLink(primaryParticipant.Id, linkedParticipantType);
            }
        }

        public void AnonymiseCaseName()
        {
            CaseName = new StringCreator().Get(9).ToUpperInvariant();
        }

        public void AnonymiseQuickLinkParticipants()
        {
            var participants = Participants
                .Where(p => p.ConferenceId == Id &&
                            p is QuickLinkParticipant)
                .ToList();

            foreach (var participant in participants)
            {
                if (participant.Username.Contains(Constants.AnonymisedUsernameSuffix)) continue;
                var randomString = new StringCreator().Get(9).ToUpperInvariant();

                participant.Username = $"{randomString}{Constants.AnonymisedUsernameSuffix}";
                participant.Name = $"{randomString} {randomString}";
                participant.DisplayName = randomString;
            }
        }

        public void AddRooms(IList<Room> rooms)
        {
            _rooms.AddRange(rooms);
        }
        
        public void AddRoom(ConsultationRoom room)
        {
            ArgumentNullException.ThrowIfNull(room);
            
            if (string.IsNullOrWhiteSpace(room.Label))
            {
                _rooms.Add(room);
                return;
            }
            
            if (_rooms.Exists(r => r.Label == room.Label && r.Status == RoomStatus.Live))
            {
                throw new DomainRuleException(nameof(room), $"Room {room.Label} already exists in conference and is still open");
            }
            _rooms.Add(room);
        }
        
        /// <summary>
        /// Add a telephone participant to the conference.
        /// </summary>
        /// <param name="telephoneParticipant">The telephone participant to add.</param>
        /// <exception cref="DomainRuleException">Thrown when the telephone participant already exists in the conference.</exception>
        public void AddTelephoneParticipant(TelephoneParticipant telephoneParticipant)
        {
            if(DoesTelephoneParticipantExist(telephoneParticipant.Id))
            {
                throw new DomainRuleException(nameof(telephoneParticipant), "Telephone participant already exists in conference");
            }
            TelephoneParticipants.Add(telephoneParticipant);
        }
        
        /// <summary>
        /// Remove a telephone participant from the conference. Marks the participant as disconnected.
        /// </summary>
        /// <param name="telephoneParticipant">The telephone participant to remove.</param>
        /// <exception cref="DomainRuleException">Thrown when the telephone participant does not exist in the conference.</exception>
        public void RemoveTelephoneParticipant(TelephoneParticipant telephoneParticipant)
        {
            if(!DoesTelephoneParticipantExist(telephoneParticipant.Id))
            {
                throw new DomainRuleException(nameof(telephoneParticipant), "Telephone participant does not exist in conference");
            }
            var existingParticipant = TelephoneParticipants.Single(x => x.Id == telephoneParticipant.Id);
            existingParticipant.UpdateCurrentRoom(null);
            existingParticipant.UpdateStatus(TelephoneState.Disconnected);
        }

        private bool DoesTelephoneParticipantExist(Guid telephoneParticipantId)
        {
            return TelephoneParticipants.Any(x => x.Id == telephoneParticipantId);
        }

        /// <summary>
        /// Get all telephone participants in the conference who are not disconnected
        /// </summary>
        /// <returns>A read-only list of telephone participants who are not disconnected.</returns>
        public IList<TelephoneParticipant> GetTelephoneParticipants()
        {
            return TelephoneParticipants.Where(x => x.State != TelephoneState.Disconnected).ToList().AsReadOnly();
        }
    }
}
