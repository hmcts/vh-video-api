using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.MagicLink
{
    public class ValidateMagicLinkTests : MagicLinkControllerTestsBase
    {
        [Test]
        public async Task Should_call_query_handler_to_getConference()
        {
            //Arrange/Act
            await Controller.ValidateMagicLink(HearingId);

            //Assert
            QueryHandler.Verify(x => x.Handle<GetConferenceByHearingRefIdQuery, VideoApi.Domain.Conference>(
                It.IsAny<GetConferenceByHearingRefIdQuery>()), Times.Once);
        }

        [Test]
        public async Task Should_return_false_ok_result_ifConference_is_null()
        {
            //Arrange
            Conference = null;
            QueryHandler.Setup(x => x.Handle<GetConferenceByHearingRefIdQuery, VideoApi.Domain.Conference>(
                It.IsAny<GetConferenceByHearingRefIdQuery>())).ReturnsAsync(Conference);

            ///Act
            var result = await Controller.ValidateMagicLink(HearingId) as OkObjectResult;

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.False((bool)result.Value);
        }

        [Test]
        public async Task Should_return_false_ok_result_ifConference_is_closed()
        {
            //Arrange
            Conference.UpdateConferenceStatus(ConferenceState.Closed);

            ///Act
            var result = await Controller.ValidateMagicLink(HearingId) as OkObjectResult;

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.False((bool)result.Value);
        }

        [Test]
        public async Task Should_return_true_ok_result_ifConference_is_closed()
        {
            //Arrange/Act
            var result = await Controller.ValidateMagicLink(HearingId) as OkObjectResult;

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.True((bool)result.Value);
        }

        [Test]
        public async Task Should_return_not_found_result_ifConference_is_not_found()
        {
            //Arrange
            QueryHandler.Setup(x => x.Handle<GetConferenceByHearingRefIdQuery, VideoApi.Domain.Conference>(
                It.IsAny<GetConferenceByHearingRefIdQuery>())).ThrowsAsync(new ConferenceNotFoundException(HearingId));

            //Act
            var result = await Controller.ValidateMagicLink(HearingId) as NotFoundObjectResult;

            //Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            Assert.False((bool)result.Value);
        }
    }
}
