using System;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Mappings;

namespace VideoApi.UnitTests.Mappings
{
    public class SharedParticipantRoomResponseMapperTests
    {
        [Test]
        public void should_map_domain_room_to_interpreter_response()
        {
            var room = Builder<Room>.CreateNew()
                .WithFactory(() => new Room(Guid.NewGuid(), VirtualCourtRoomType.Civilian, false)).Build();
            var response = SharedParticipantRoomResponseMapper.MapRoomToResponse(room);

            response.Label.Should().Be(room.Label);
            response.ParticipantJoinUri.Should().Be(room.ParticipantUri);
            response.PexipNode.Should().Be(room.PexipNode);
            response.RoomType.ToString().Should().Be(room.Type.ToString());
        }
    }
}
