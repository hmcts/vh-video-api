using VideoApi.Contract.Requests;
using VideoApi.Mappings;
using VideoApi.Services.Clients;

namespace VideoApi.UnitTests.Mappings
{
    public class HearingLayoutMapperTests
    {
        [TestCase(HearingLayout.Dynamic, Layout.AUTOMATIC)]
        [TestCase(HearingLayout.OnePlus7, Layout.ONE_PLUS_SEVEN)]
        [TestCase(HearingLayout.TwoPlus21, Layout.TWO_PLUS_TWENTYONE)]
        public void should_map_to_hearing_request(HearingLayout hearingLayout, Layout layout)
        {
            HearingLayoutMapper.MapLayoutToVideoHearingLayout(hearingLayout).Should().Be(layout);
        }
    }
}
