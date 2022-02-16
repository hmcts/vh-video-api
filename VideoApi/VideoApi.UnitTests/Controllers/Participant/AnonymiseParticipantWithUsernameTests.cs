using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;

namespace VideoApi.UnitTests.Controllers.Participant
{
    public class AnonymiseParticipantWithUsernameTests : ParticipantsControllerTestBase
    {
        [Test]
        public async Task Returns_Ok_For_Successful_Request()
        {
            var usernameToAnonymise = "john.doe@email.net";

            var response = await Controller.AnonymiseParticipantWithUsername(usernameToAnonymise) as OkResult;

            response.StatusCode.Should().Be((int) HttpStatusCode.OK);
            MockCommandHandler.Verify(
                commandHandler => commandHandler.Handle(
                    It.Is<AnonymiseParticipantWithUsernameCommand>(command => command.Username == usernameToAnonymise)),
                Times.Once);
        }

        [Test]
        public async Task Returns_Not_Found_For_Invalid_Username()
        {
            var username = "user@email.com";
            var exception = new ParticipantNotFoundException(username);

            MockCommandHandler
                .Setup(commandHandler => commandHandler.Handle(It.IsAny<AnonymiseParticipantWithUsernameCommand>()))
                .ThrowsAsync(exception);
            
            var response = await Controller.AnonymiseParticipantWithUsername(username) as NotFoundResult;
            
            response.StatusCode.Should().Be((int) HttpStatusCode.NotFound);
            
            _mockLogger.Verify(x => x.Log(
                It.Is<LogLevel>(log => log == LogLevel.Error),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.Is<Exception>(x => x == exception),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }
    }
}
