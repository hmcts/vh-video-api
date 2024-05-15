using Microsoft.AspNetCore.Mvc;
using Moq;
using VideoApi.DAL.Queries;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.QuickLink
{
    public class ValidateQuickLinkTests : QuickLinkControllerTestsBase
    {
        [Test]
        public async Task Should_call_query_handler_to_getConference()
        {
            //Arrange/Act
            await Controller.ValidateQuickLink(HearingId);

            //Assert
            QueryHandler.Verify(x => x.Handle<GetConferenceByHearingRefIdQuery, VideoApi.Domain.Conference>(
                It.IsAny<GetConferenceByHearingRefIdQuery>()), Times.Once);
        }

        [Test]
        public async Task Should_return_false_ok_result_if_conference_is_null()
        {
            //Arrange
            Conference = null;
            QueryHandler.Setup(x => x.Handle<GetConferenceByHearingRefIdQuery, VideoApi.Domain.Conference>(
                It.IsAny<GetConferenceByHearingRefIdQuery>())).ReturnsAsync(Conference);

            ///Act
            var result = await Controller.ValidateQuickLink(HearingId) as OkObjectResult;

            //Assert
            ClassicAssert.IsInstanceOf<OkObjectResult>(result);
            ClassicAssert.False((bool)result.Value);
        }

        [Test]
        public async Task Should_return_true_ok_result()
        {
            //Arrange/Act
            var result = await Controller.ValidateQuickLink(HearingId) as OkObjectResult;

            //Assert
            ClassicAssert.IsInstanceOf<OkObjectResult>(result);
            ClassicAssert.True((bool)result.Value);
        }
    }
}
