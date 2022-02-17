using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;

namespace VideoApi.UnitTests.Controllers.Conference
{
    public class AnonymiseConferenceWithHearingIdsTests : ConferenceControllerTestBase
    {
        [Test]
        public async Task Returns_Ok_For_Successful_Request()
        {
            var hearingIds = new List<Guid> {Guid.NewGuid()};

            var response = await Controller.AnonymiseConferenceWithHearingIds(hearingIds) as OkResult;

            response.StatusCode.Should().Be((int) HttpStatusCode.OK);
            CommandHandlerMock.Verify(
                commandHandler => commandHandler.Handle(
                    It.Is<AnonymiseConferenceWithHearingIdsCommand>(command => command.HearingIds == hearingIds)),
                Times.Once);
        }
        
        [Test]
        public async Task Returns_Not_Found_For_Invalid_HearingIds()
        {
            var hearingIds = new List<Guid> {Guid.NewGuid()};
            var exception = new ConferenceNotFoundException(hearingIds);

            CommandHandlerMock
                .Setup(commandHandler => commandHandler.Handle(It.IsAny<AnonymiseConferenceWithHearingIdsCommand>()))
                .ThrowsAsync(exception);
            
            var response = await Controller.AnonymiseConferenceWithHearingIds(hearingIds) as NotFoundResult;
            
            response.StatusCode.Should().Be((int) HttpStatusCode.NotFound);
            
            MockLogger.Verify(x => x.Log(
                It.Is<LogLevel>(log => log == LogLevel.Error),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.Is<Exception>(x => x == exception),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }
    }
}
