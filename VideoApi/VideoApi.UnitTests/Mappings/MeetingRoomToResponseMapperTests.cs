using VideoApi.Domain;
using VideoApi.Mappings;

namespace VideoApi.UnitTests.Mappings
{
    public class MeetingRoomToResponseMapperTests
    {
        [Test]
        public void Should_map_all_properties()
        {
            const string adminUri = "https://testpoc.node.com/viju/#/?conference=user@hmcts.net&output=embed";
            const string judgeUri = "https://judgetestpoc.node.com/viju/#/?conference=user@hmcts.net&output=embed";
            const string participantUri =
                "https://participantstestpoc.node.com/viju/#/?conference=user@hmcts.net&output=embed";
            const string pexipNode = "testjoin.node.com";
            const string telephoneConferenceId = "12345678";

            var meetingRoom = new MeetingRoom(adminUri, judgeUri, participantUri, pexipNode, telephoneConferenceId);

            var response = MeetingRoomToResponseMapper.MapVirtualCourtToResponse(meetingRoom);
            response.Should().BeEquivalentTo(meetingRoom);
        }
    }
}
