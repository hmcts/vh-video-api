using System;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Domain.Rooms
{
    public class InterpreterRoomTests
    {
        [Test]
        public void should_update_room_connection_details()
        {
            var room = new ParticipantRoom(Guid.NewGuid(), VirtualCourtRoomType.Civilian);
            room.Label.Should().BeNull();

            var label = "Interpreter1";
            var ingestUrl = $"rtmps://hostserver/hearingId1/hearingId1/{room.Id}";
            var node = "sip.test.com";
            var participantUri = "env-foo-interpeterroom";
            
            room.UpdateConnectionDetails(label, ingestUrl, node, participantUri);

            room.Label.Should().Be(label);
            room.IngestUrl.Should().Be(ingestUrl);
            room.PexipNode.Should().Be(node);
            room.ParticipantUri.Should().Be(participantUri);
        }
    }
}
