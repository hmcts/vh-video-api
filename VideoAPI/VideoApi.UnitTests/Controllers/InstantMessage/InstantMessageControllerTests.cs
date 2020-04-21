using System;
using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Video.API.Controllers;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries.Core;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.InstantMessage
{
    public class InstantMessageControllerTests
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
                MessageText = "Test message text"
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
    }
}
