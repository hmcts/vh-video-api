using System;
using VideoApi.Domain.Ddd;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;

namespace VideoApi.Domain
{
    public class Endpoint : TrackableEntity<Guid>
    {
        public string DisplayName { get; private set; }
        public string SipAddress { get; }
        public string Pin { get; }
        public EndpointState State { get; private set; }
        public string DefenceAdvocate { get; private set; }
        public RoomType? CurrentRoom { get; private set; }
        public long? CurrentConsultationRoomId { get; set; }
        public virtual ConsultationRoom CurrentConsultationRoom { get; set; }
        public ConferenceRole ConferenceRole { get; private set; }

        private Endpoint()
        {
            Id = Guid.NewGuid();
            State = EndpointState.NotYetJoined;
            ConferenceRole = ConferenceRole.Host;
        }

        public Endpoint(string displayName, string sipAddress, string pin, string defenceAdvocate, ConferenceRole conferenceRole = ConferenceRole.Host) : this()
        {
            DisplayName = displayName;
            SipAddress = sipAddress;
            Pin = pin;
            DefenceAdvocate = defenceAdvocate;
            ConferenceRole = conferenceRole;
        }

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

        public void AssignDefenceAdvocate(string username)
        {
            DefenceAdvocate = username;
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
