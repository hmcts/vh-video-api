using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Domain.Endpoints
{
    public class UpdateRoomTests
    {
        [TestCase(RoomType.WaitingRoom)]
        [TestCase(RoomType.HearingRoom)]
        [TestCase(RoomType.ConsultationRoom)]
        [TestCase(RoomType.AdminRoom)]
        [TestCase(null)]
        public void should_update_room(RoomType? newRoom)
        {
            var endpoint = new Endpoint("old name", "123@sip.com", "1234");
            
            endpoint.UpdateCurrentRoom(newRoom);

            endpoint.CurrentRoom.Should().Be(newRoom);
        }
    }
}
