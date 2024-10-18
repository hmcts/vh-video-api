using System;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Domain.TelephoneParticipants;

public class UpdateCurrentRoomTests
{
    [Test]
    public void should_update_current_room()
    {
        var conference = new ConferenceBuilder().Build();
        var telephoneParticipant = new TelephoneParticipant(Guid.NewGuid(), "Anonymous", conference);
        var room = RoomType.WaitingRoom;
        telephoneParticipant.UpdateCurrentRoom(room);
        telephoneParticipant.CurrentRoom.Should().Be(room);
    }
}
