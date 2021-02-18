using System;
using VideoApi.Domain.Ddd;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;

namespace VideoApi.Domain
{
    public class Endpoint : Entity<Guid>
    {
        public string DisplayName { get; private set; }
        public string SipAddress { get; }
        public string Pin { get; }
        public EndpointState State { get; private set; }
        public string DefenceAdvocate { get; private set; }
        public RoomType? CurrentRoom { get; private set; }
        public long? CurrentVirtualRoomId { get; set; }
        public virtual Room CurrentVirtualRoom { get; set; }

        private Endpoint()
        {
            Id = Guid.NewGuid();
            State = EndpointState.NotYetJoined;
        }

        public Endpoint(string displayName, string sipAddress, string pin, string defenceAdvocate) : this()
        {
            DisplayName = displayName;
            SipAddress = sipAddress;
            Pin = pin;
            DefenceAdvocate = defenceAdvocate;
        }

        public void UpdateDisplayName(string displayName)
        {
            DisplayName = displayName;
        }

        public void UpdateStatus(EndpointState status)
        {
            State = status;
        }

        public void AssignDefenceAdvocate(string username)
        {
            DefenceAdvocate = username;
        }

        public string GetCurrentRoom()
        {
            return CurrentVirtualRoom?.Label ?? CurrentRoom?.ToString() ?? throw new DomainRuleException(nameof(CurrentRoom), "Endpoint is not in a room");
        }

        public void UpdateCurrentRoom(RoomType? currentRoom)
        {
            CurrentRoom = currentRoom;
        }

        public void UpdateCurrentVirtualRoom(Room room)
        {
            CurrentVirtualRoom = room;
        }
    }
}
