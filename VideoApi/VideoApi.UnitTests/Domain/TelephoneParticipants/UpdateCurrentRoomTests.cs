using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Domain.TelephoneParticipants;

public class UpdateCurrentRoomTests
{
    [Test]
    public void should_update_current_room()
    {
        var telephoneParticipant = new TelephoneParticipant("Anonymous");
        var room = RoomType.WaitingRoom;
        telephoneParticipant.UpdateCurrentRoom(room);
        telephoneParticipant.CurrentRoom.Should().Be(room);
    }
}
