using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Controllers;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.MagicLink
{
    public class ValidateMagicLinkTests
    {
        private Mock<IQueryHandler> _queryHandler;
        private Mock<ICommandHandler> _commandHandler;
        private Mock<ILogger<MagicLinksController>> _logger;

        private MagicLinksController _controller;

        private Guid _hearingId;
        private VideoApi.Domain.Conference _conference;
        
        [SetUp]
        public void SetUp()
        {
            _queryHandler = new Mock<IQueryHandler>();
            _commandHandler = new Mock<ICommandHandler>();
            _logger = new Mock<ILogger<MagicLinksController>>();


            _conference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Applicant", null, null, RoomType.ConsultationRoom)
                .WithParticipant(UserRole.Representative, "Applicant")
                .Build();
            
            _queryHandler.Setup(x => x.Handle<GetConferenceByHearingRefIdQuery, VideoApi.Domain.Conference>(
                It.IsAny<GetConferenceByHearingRefIdQuery>())).ReturnsAsync(_conference);

            _hearingId = Guid.NewGuid();
         
            _controller = new MagicLinksController(_commandHandler.Object, _queryHandler.Object, _logger.Object);
        }

        [Test]
        public async Task Should_call_query_handler_to_get_conference()
        {
            //Arrange/Act
            await _controller.ValidateMagicLink(_hearingId);

            //Assert
            _queryHandler.Verify(x => x.Handle<GetConferenceByHearingRefIdQuery, VideoApi.Domain.Conference>(
                It.IsAny<GetConferenceByHearingRefIdQuery>()), Times.Once);
        }

        [Test]
        public async Task Should_return_false_ok_result_if_conference_is_null()
        {
            //Arrange
            _conference = null;
            _queryHandler.Setup(x => x.Handle<GetConferenceByHearingRefIdQuery, VideoApi.Domain.Conference>(
                It.IsAny<GetConferenceByHearingRefIdQuery>())).ReturnsAsync(_conference);

            ///Act
            var result = await _controller.ValidateMagicLink(_hearingId) as OkObjectResult;

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.False((bool)result.Value);
        }

        [Test]
        public async Task Should_return_false_ok_result_if_conference_is_closed()
        {
            //Arrange
            _conference.UpdateConferenceStatus(ConferenceState.Closed);

            ///Act
            var result = await _controller.ValidateMagicLink(_hearingId) as OkObjectResult;

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.False((bool)result.Value);
        }

        [Test]
        public async Task Should_return_true_ok_result_if_conference_is_closed()
        {
            //Arrange/Act
            var result = await _controller.ValidateMagicLink(_hearingId) as OkObjectResult;

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.True((bool)result.Value);
        }

        [Test]
        public async Task Should_return_not_found_result_if_conference_is_not_found()
        {
            //Arrange
            _queryHandler.Setup(x => x.Handle<GetConferenceByHearingRefIdQuery, VideoApi.Domain.Conference>(
                It.IsAny<GetConferenceByHearingRefIdQuery>())).ThrowsAsync(new ConferenceNotFoundException(_hearingId));

            //Act
            var result = await _controller.ValidateMagicLink(_hearingId) as NotFoundObjectResult;

            //Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            Assert.False((bool)result.Value);
        }
    }
}
