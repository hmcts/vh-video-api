using System;
using VideoApi.Domain.Enums;

namespace VideoApi.Domain;

public class TelephoneParticipant: TrackableEntity<Guid>
{
    public TelephoneState State { get; private set; }
    public RoomType? CurrentRoom { get; private set; }
    public string TelephoneNumber { get; private set; }
    
    public TelephoneParticipant(string telephoneNumber)
    {
        Id = Guid.NewGuid();
        State = TelephoneState.Connected;
        CurrentRoom = RoomType.HearingRoom;
        TelephoneNumber = telephoneNumber;
    }
    
    public void UpdateStatus(TelephoneState status)
    {
        State = status;
    }
    
    public void UpdateCurrentRoom(RoomType? currentRoom)
    {
        CurrentRoom = currentRoom;
    }
}
