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
        public async Task Should_successfully_save_given_message_request_and_return_ok_result()
        {
            var request = new AddInstantMessageRequest
            {
                From = "Display From",
                MessageText = "Test message text"
            };

            var result = await _instantMessageController.AddInstantMessageToConference(Guid.NewGuid(), request);

            var typedResult = (OkObjectResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            typedResult.Value.Should().Be("InstantMessage saved");
            _commandHandler.Verify(c => c.Handle(It.IsAny<AddInstantMessageCommand>()), Times.Once);
        }
    }
}
