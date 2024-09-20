using System;
using VideoApi.Domain.Enums;

namespace VideoApi.Domain;

public class TelephoneParticipant: TrackableEntity<Guid>
{
    public Guid ConferenceId { get; set; }
    public Conference Conference { get; set; }
    
    public TelephoneState State { get; set; }
    public RoomType? CurrentRoom { get; set; }
    public string TelephoneNumber { get; private set; }
    
    public TelephoneParticipant(Guid id, string telephoneNumber)
    {
        Id = id;
        State = TelephoneState.Connected;
        CurrentRoom = RoomType.WaitingRoom;
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
