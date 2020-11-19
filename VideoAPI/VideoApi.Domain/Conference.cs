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
            InstantMessageHistory = new List<InstantMessage>();
            MeetingRoom = new MeetingRoom();
            Endpoints = new List<Endpoint>();

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
        public DateTime? ActualStartTime { get; private set; }
        public DateTime? ClosedDateTime { get; private set; }
        public string CaseNumber { get; private set; }
        public string CaseName { get; private set; }
        public MeetingRoom MeetingRoom { get; private set; }
        public int ScheduledDuration { get; set; }
        public ConferenceState State { get; private set; }
        public virtual IList<Participant> Participants { get; }
        public virtual IList<Endpoint> Endpoints { get; }
        public virtual IList<ConferenceStatus> ConferenceStatuses { get; }
        public virtual IList<InstantMessage> InstantMessageHistory { get; }
        public string HearingVenueName { get; private set; }
        public bool AudioRecordingRequired { get; set; }
        public string IngestUrl { get; set; }

        public void UpdateMeetingRoom(string adminUri, string judgeUri, string participantUri, string pexipNode,
            string telephoneConferenceId)
        {
            MeetingRoom = new MeetingRoom(adminUri, judgeUri, participantUri, pexipNode, telephoneConferenceId);
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

            var endpointRooms = GetEndpoints().Select(x => x.CurrentRoom);
            var participantRooms = GetParticipants().Select(x => x.CurrentRoom);
            var allRooms = new List<RoomType?>(endpointRooms).Concat(participantRooms).ToList();
            
            var consultationRoomOneOccupied = allRooms.Any(x => x == RoomType.ConsultationRoom1);
            if (!consultationRoomOneOccupied)
            {
                return RoomType.ConsultationRoom1;
            }

            var consultationRoomTwoOccupied = allRooms.Any(x => x == RoomType.ConsultationRoom2);
            if (!consultationRoomTwoOccupied)
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
    }
}
