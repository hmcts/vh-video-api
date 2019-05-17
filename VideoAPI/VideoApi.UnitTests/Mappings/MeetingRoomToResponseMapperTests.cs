using FluentAssertions;
using NUnit.Framework;
using Video.API.Mappings;
using VideoApi.Domain;

namespace VideoApi.UnitTests.Mappings
{
    public class MeetingRoomToResponseMapperTests
    {
        private readonly MeetingRoomToResponseMapper _mapper = new MeetingRoomToResponseMapper();

        [Test]
        public void should_map_all_properties()
        {
            const string adminUri = "https://testpoc.node.com/viju/#/?conference=user@email.com&output=embed";
            const string judgeUri = "https://judgetestpoc.node.com/viju/#/?conference=user@email.com&output=embed";
            const string participantUri =
                "https://participantstestpoc.node.com/viju/#/?conference=user@email.com&output=embed";
            const string pexipNode = "testjoin.node.com";

            var meetingRoom = new MeetingRoom(adminUri, judgeUri, participantUri, pexipNode);

            var response = _mapper.MapVirtualCourtToResponse(meetingRoom);
            response.Should().BeEquivalentTo(meetingRoom);
        }
    }
}