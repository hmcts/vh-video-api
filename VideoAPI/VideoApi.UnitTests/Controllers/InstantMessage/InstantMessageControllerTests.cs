using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using Testing.Common.Helper.Builders.Domain;
using Video.API.Controllers;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.UnitTests.Controllers.Conference;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.InstantMessage
{
    public class InstantMessageControllerTests : ConferenceControllerTestBase
    {
        private Mock<IQueryHandler> _queryHandler;
        private Mock<ICommandHandler> _commandHandler;
        private Mock<ILogger<InstantMessageController>> _logger;
        private InstantMessageController _instantMessageController;

        [SetUp]
        public void TestInitialize()
        {
            _queryHandler = new Mock<IQueryHandler>();
            _commandHandler = new Mock<ICommandHandler>();
            _logger = new Mock<ILogger<InstantMessageController>>();

            _instantMessageController = new InstantMessageController(_queryHandler.Object,_commandHandler.Object,_logger.Object);
        }

        [Test]
        public async Task Should_successfully_save_given_instantmessages_request_and_return_ok_result()
        {
            var request = new AddInstantMessageRequest
            {
                From = "Display From",
                MessageText = "Test message text",
                To = "Display To"
            };

            var result = await _instantMessageController.AddInstantMessageToConferenceAsync(Guid.NewGuid(), request);

            var typedResult = (OkObjectResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            typedResult.Value.Should().Be("InstantMessage saved");
            _commandHandler.Verify(c => c.Handle(It.IsAny<AddInstantMessageCommand>()), Times.Once);
        }
        
        [Test]
        public async Task Should_successfully_remove_instantmessages_and_return_nocontent()
        {
            var result = await _instantMessageController.RemoveInstantMessagesForConferenceAsync(Guid.NewGuid());

            var typedResult = (NoContentResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
            _commandHandler.Verify(c => c.Handle(It.IsAny<RemoveInstantMessagesForConferenceCommand>()), Times.Once);
        }
        
        [Test]
        public async Task Should_return_badRequest()
        {
            _commandHandler.Setup(x => x.Handle(It.IsAny<RemoveInstantMessagesForConferenceCommand>()))
                .Throws(new Exception());
            
            var result = await _instantMessageController.RemoveInstantMessagesForConferenceAsync(Guid.NewGuid());

            var typedResult = (BadRequestResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            _commandHandler.Verify(c => c.Handle(It.IsAny<RemoveInstantMessagesForConferenceCommand>()), Times.Once);
        }

        [Test]
        public async Task Should_return_ok_result_for_given_conference_id_and_participantname()
        {
            var instantMessages = new List<VideoApi.Domain.InstantMessage>
            {
                new VideoApi.Domain.InstantMessage("Individual01", "test message 01", "VH Officer") { ConferenceId = Guid.NewGuid() },
                new VideoApi.Domain.InstantMessage("VH Officer", "test message 02", "Individual01") { ConferenceId = Guid.NewGuid() }
            };

            _queryHandler
                .Setup(x => x.Handle<GetInstantMessagesForConferenceQuery, List<VideoApi.Domain.InstantMessage>>(It.IsAny<GetInstantMessagesForConferenceQuery>()))
                .ReturnsAsync(instantMessages);
            var result = await _instantMessageController.GetInstantMessageHistoryAsync(Guid.NewGuid(), "VH Officer");
            var typedResult = (OkObjectResult)result;
            typedResult.Should().NotBeNull();
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Test]
        public async Task Should_return_not_found_result_for_given_conference_id_and_participantname()
        {
            _queryHandler
                .Setup(x => x.Handle<GetInstantMessagesForConferenceQuery, List<VideoApi.Domain.InstantMessage>>(It.IsAny<GetInstantMessagesForConferenceQuery>()))
                .ReturnsAsync((List<VideoApi.Domain.InstantMessage>)null);
            var result = await _instantMessageController.GetInstantMessageHistoryAsync(Guid.NewGuid(), "VH Officer");
            var typedResult = (NotFoundResult)result;
            typedResult.Should().NotBeNull();
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Should_return_empty_result_for_if_no_closed_conference_exist()
        {
            _queryHandler
                .Setup(x => x.Handle<GetClosedConferencesWithInstantMessagesQuery, List<VideoApi.Domain.Conference>>(It.IsAny<GetClosedConferencesWithInstantMessagesQuery>()))
                .ReturnsAsync(new List<VideoApi.Domain.Conference>());
            var result = await _instantMessageController.GetClosedConferencesWithInstantMessagesAsync();
            var typedResult = (OkObjectResult)result;
            typedResult.Should().NotBeNull();
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Test]
        public async Task Should_return_ok_result_when_get_closed_conference_with_im_called()
        {
            var conferenceType = typeof(VideoApi.Domain.Conference);
            var utcDate = DateTime.UtcNow;
            var oldHearing = utcDate.AddMinutes(-180);

            var closedConf = new ConferenceBuilder(true, scheduledDateTime: oldHearing)
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithParticipant(UserRole.Judge, null)
                .WithMessages(10)
                .WithConferenceStatus(ConferenceState.Closed)
                .Build();
            conferenceType.GetProperty("ClosedDateTime").SetValue(closedConf, DateTime.UtcNow.AddMinutes(-31));

            var closedConferences = new List<VideoApi.Domain.Conference>();
            closedConferences.Add(closedConf);

            _queryHandler
                .Setup(x => x.Handle<GetClosedConferencesWithInstantMessagesQuery, List<VideoApi.Domain.Conference>>(It.IsAny<GetClosedConferencesWithInstantMessagesQuery>()))
                .ReturnsAsync(closedConferences);
            var result = await _instantMessageController.GetClosedConferencesWithInstantMessagesAsync();
            var typedResult = (OkObjectResult)result;
            typedResult.Should().NotBeNull();
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }
    }
}
