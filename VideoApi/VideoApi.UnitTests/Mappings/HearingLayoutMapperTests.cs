using VideoApi.Contract.Requests;
using VideoApi.Mappings;
using VideoApi.Services.Clients.Models;

namespace VideoApi.UnitTests.Mappings
{
    public class HearingLayoutMapperTests
    {
        [TestCase(HearingLayout.Dynamic, Layout.Automatic)]
        [TestCase(HearingLayout.OnePlus7, Layout.OnePlusSeven)]
        [TestCase(HearingLayout.TwoPlus21, Layout.TwoPlusTwentyone)]
        [TestCase(HearingLayout.NineEqual, Layout.NineEqual)]
        [TestCase(HearingLayout.SixteenEqual, Layout.SixteenEqual)]
        [TestCase(HearingLayout.TwentyFiveEqual, Layout.TwentyFiveEqual)]
        public void should_map_to_hearing_request(HearingLayout hearingLayout, Layout layout)
        {
            HearingLayoutMapper.MapLayoutToVideoHearingLayout(hearingLayout).Should().Be(layout);
        }
    }
}
