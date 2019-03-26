using FluentAssertions;
using NUnit.Framework;
using Video.API.Mappings;
using VideoApi.Domain;

namespace VideoApi.UnitTests.Mappings
{
    public class VirtualCourtToResponseMapperTests
    {
        private readonly VirtualCourtToResponseMapper _mapper = new VirtualCourtToResponseMapper();

        [Test]
        public void should_map_all_properties()
        {
            var adminUri = "https://testjoin.poc.hearings.hmcts.net/viju/#/?conference=ola@hearings.hmcts.net&output=embed";
            var judgeUri = "https://testjoin.poc.hearings.hmcts.net/viju/#/?conference=ola@hearings.hmcts.net&output=embed";
            var participantUri =
                "https://testjoin.poc.hearings.hmcts.net/viju/#/?conference=ola@hearings.hmcts.net&output=embed";
            var pexipNode = "testjoin.poc.hearings.hmcts.net";
            
            var virtualCourt = new VirtualCourt(adminUri, judgeUri, participantUri, pexipNode);
            
            var response = _mapper.MapVirtualCourtToResponse(virtualCourt);
            response.Should().BeEquivalentTo(virtualCourt, options => options
                .Excluding(court => court.Id));
        }
    }
}