using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;

namespace VideoApi.UnitTests.Domain.Conference
{
    public class UpdateVirtualCourtTests
    {
        [Test]
        public void should_update_existing_virtual_court_details()
        {
            var conference = new ConferenceBuilder().WithVirtualCourt().Build();
            conference.GetVirtualCourt().Should().NotBeNull();
            
            var adminUri = "https://testjoin.poc.hearings.hmcts.net/viju/#/?conference=ola@hearings.hmcts.net&output=embed";
            var judgeUri = "https://testjoin.poc.hearings.hmcts.net/viju/#/?conference=ola@hearings.hmcts.net&output=embed";
            var participantUri =
                "https://testjoin.poc.hearings.hmcts.net/viju/#/?conference=ola@hearings.hmcts.net&output=embed";
            var pexipNode = "testjoin.poc.hearings.hmcts.net";
            conference.UpdateVirtualCourt(adminUri, judgeUri, participantUri, pexipNode);

            var virtualCourt = conference.GetVirtualCourt();
            virtualCourt.AdminUri.Should().Be(adminUri);
            virtualCourt.JudgeUri.Should().Be(judgeUri);
            virtualCourt.ParticipantUri.Should().Be(participantUri);
            virtualCourt.PexipNode.Should().Be(pexipNode);
        }
        
        [Test]
        public void should_add_virtual_court_details()
        {
            var conference = new ConferenceBuilder().Build();
            conference.GetVirtualCourt().Should().BeNull();
            
            var adminUri = "https://join.poc.hearings.hmcts.net/viju/#/?conference=ola@hearings.hmcts.net&output=embed";
            var judgeUri = "https://join.poc.hearings.hmcts.net/viju/#/?conference=ola@hearings.hmcts.net&output=embed";
            var participantUri =
                "https://join.poc.hearings.hmcts.net/viju/#/?conference=ola@hearings.hmcts.net&output=embed";
            var pexipNode = "join.poc.hearings.hmcts.net";
            conference.UpdateVirtualCourt(adminUri, judgeUri, participantUri, pexipNode);

            var virtualCourt = conference.GetVirtualCourt();
            virtualCourt.AdminUri.Should().Be(adminUri);
            virtualCourt.JudgeUri.Should().Be(judgeUri);
            virtualCourt.ParticipantUri.Should().Be(participantUri);
            virtualCourt.PexipNode.Should().Be(pexipNode);
        }
    }
}