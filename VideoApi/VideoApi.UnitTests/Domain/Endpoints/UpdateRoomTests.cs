using FluentAssertions;
using NUnit.Framework;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Domain.Endpoints
{
    public class UpdateRoomTests
    {
        [TestCase(RoomType.WaitingRoom)]
        [TestCase(RoomType.HearingRoom)]
        [TestCase(RoomType.ConsultationRoom1)]
        [TestCase(RoomType.ConsultationRoom2)]
        [TestCase(RoomType.AdminRoom)]
        [TestCase(null)]
        public void should_update_room(RoomType? newRoom)
        {
            var endpoint = new Endpoint("old name", "123@sip.com", "1234", "defence@sol.com");
            
            endpoint.UpdateCurrentRoom(newRoom);

            endpoint.CurrentRoom.Should().Be(newRoom);
        }
    }
}
