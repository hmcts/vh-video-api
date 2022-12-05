using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.Controllers;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
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
        private readonly BackgroundWorkerQueue _backgroundWorkerQueue = new BackgroundWorkerQueue();

        [SetUp]
        public void TestInitialize()
        {
            _queryHandler = new Mock<IQueryHandler>();
            _commandHandler = new Mock<ICommandHandler>();
            _logger = new Mock<ILogger<InstantMessageController>>();

            _instantMessageController = new InstantMessageController(_queryHandler.Object,_commandHandler.Object,_logger.Object, _backgroundWorkerQueue);
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

            var typedResult = (OkResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
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
        public async Task Should_return_Ok_and_an_empty_list_when_no_closed_conferences_found()
        {
            var closedConferences = new List<VideoApi.Domain.Conference>();
            _queryHandler
                .Setup(x => x.Handle<GetClosedConferencesWithInstantMessagesQuery, List<VideoApi.Domain.Conference>>(It.IsAny<GetClosedConferencesWithInstantMessagesQuery>()))
                .ReturnsAsync(closedConferences);

            var result = await _instantMessageController.GetClosedConferencesWithInstantMessagesAsync();
            var typedResult = (OkObjectResult)result;
            typedResult.Should().NotBeNull();
            var response = (List<ClosedConferencesResponse>) typedResult.Value;
            response.Should().NotBeNull();
            response.Count.Should().Be(0);
        }
    }
}
