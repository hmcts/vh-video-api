using System;
using System.Collections.Generic;
using System.Linq;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;

namespace VideoApi.Domain
{
    public class Endpoint : TrackableEntity<Guid>
    {
        private Endpoint()
        {
            Id = Guid.NewGuid();
            State = EndpointState.NotYetJoined;
            ConferenceRole = ConferenceRole.Host;
        }

        public Endpoint(string displayName, string sipAddress, string pin, ConferenceRole conferenceRole = ConferenceRole.Host) : this()
        {
            DisplayName = displayName;
            SipAddress = sipAddress;
            Pin = pin;
            ConferenceRole = conferenceRole;
        }

        public string DisplayName { get; private set; }
        public string SipAddress { get; }
        public string Pin { get; }
        public EndpointState State { get; private set; }

        [Obsolete("This property is not used and will be removed in the future")]
        public string DefenceAdvocate { get; }

        public RoomType? CurrentRoom { get; private set; }
        public long? CurrentConsultationRoomId { get; set; }
        public virtual ConsultationRoom CurrentConsultationRoom { get; set; }
        public ConferenceRole ConferenceRole { get; private set; }
        public virtual IList<ParticipantBase> ParticipantsLinked { get; set; } = new List<ParticipantBase>();

        public void UpdateDisplayName(string displayName)
        {
            DisplayName = displayName;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateStatus(EndpointState status)
        {
            State = status;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateConferenceRole(ConferenceRole newConferenceRole)
        {
            ConferenceRole = newConferenceRole;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddParticipantLink(ParticipantBase participant)
        {
            ArgumentNullException.ThrowIfNull(participant);
            
            participant.Endpoint = this;
            participant.EndpointId = Id;
            ParticipantsLinked.Add(participant);
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveParticipantLink(ParticipantBase participant)
        {
            var linkedParticipant = ParticipantsLinked.FirstOrDefault(x => x.Id == participant.Id);
            if (linkedParticipant == null)
                return;

            ParticipantsLinked.Remove(linkedParticipant);
            UpdatedAt = DateTime.UtcNow;
        }

        public string GetCurrentRoom()
        {
            return CurrentConsultationRoom?.Label ?? CurrentRoom?.ToString() ?? throw new DomainRuleException(nameof(CurrentRoom), "Endpoint is not in a room");
        }

        public void UpdateCurrentRoom(RoomType? currentRoom)
        {
            CurrentRoom = currentRoom;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateCurrentVirtualRoom(ConsultationRoom consultationRoom)
        {
            CurrentConsultationRoom?.RemoveEndpoint(new RoomEndpoint(Id));
            CurrentConsultationRoom = consultationRoom;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
