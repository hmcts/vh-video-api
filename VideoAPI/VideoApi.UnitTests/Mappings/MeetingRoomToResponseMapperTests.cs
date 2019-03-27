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
            var adminUri =
                "https://testjoin.admin.poc.hearings.hmcts.net/viju/#/?conference=ola@hearings.hmcts.net&output=embed";
            var judgeUri =
                "https://testjoin.judge.poc.hearings.hmcts.net/viju/#/?conference=ola@hearings.hmcts.net&output=embed";
            var participantUri =
                "https://testjoin.poc.participants.hearings.hmcts.net/viju/#/?conference=ola@hearings.hmcts.net&output=embed";
            var pexipNode = "testjoin.node.poc.hearings.hmcts.net";

            var meetingRoom = new MeetingRoom(adminUri, judgeUri, participantUri, pexipNode);

            var response = _mapper.MapVirtualCourtToResponse(meetingRoom);
            response.Should().BeEquivalentTo(meetingRoom);
        }
    }
}