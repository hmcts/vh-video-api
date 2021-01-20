using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;

namespace VideoApi.UnitTests.Domain.Endpoints
{
    public class GetCurrentRoomTests
    {
        [TestCase(RoomType.WaitingRoom)]
        [TestCase(RoomType.HearingRoom)]
        [TestCase(RoomType.ConsultationRoom)]
        [TestCase(RoomType.AdminRoom)]
        public void should_get_current_room(RoomType newRoom)
        {
            var endpoint = new Endpoint("old name", "123@sip.com", "1234", "defence@sol.com");
            endpoint.UpdateCurrentRoom(newRoom);
            endpoint.GetCurrentRoom().Should().Be(newRoom);
        }

        [Test]
        public void should_throw_exception_when_endpoint_is_not_in_a_room()
        {
            var endpoint = new Endpoint("old name", "123@sip.com", "1234", "defence@sol.com");
            endpoint.UpdateCurrentRoom(null);
            Assert.Throws<DomainRuleException>(() => endpoint.GetCurrentRoom()).ValidationFailures
                .Any(x => x.Message == "Endpoint is not in a room").Should().BeTrue();
        }
    }
}
