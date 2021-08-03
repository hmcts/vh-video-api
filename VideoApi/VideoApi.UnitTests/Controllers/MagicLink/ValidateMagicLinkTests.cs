using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using Testing.Common.Helper.Builders.Domain;
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
        public async Task Should_return_false_ok_result_if_conference_is_null()
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
        public async Task Should_return_true_ok_result()
        {
            //Arrange/Act
            var result = await Controller.ValidateMagicLink(HearingId) as OkObjectResult;

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.True((bool)result.Value);
        }
    }
}
