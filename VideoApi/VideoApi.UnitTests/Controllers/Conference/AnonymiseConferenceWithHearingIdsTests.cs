using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;

namespace VideoApi.UnitTests.Controllers.Conference
{
    public class AnonymiseConferenceWithHearingIdsTests : ConferenceControllerTestBase
    {
        [Test]
        public async Task Returns_Ok_For_Successful_Request()
        {
            var request = new AnonymiseConferenceWithHearingIdsRequest{ HearingIds = new List<Guid> {Guid.NewGuid()}} ;

            var response = await Controller.AnonymiseConferenceWithHearingIds(request) as OkResult;

            response.StatusCode.Should().Be((int) HttpStatusCode.OK);
            CommandHandlerMock.Verify(
                commandHandler => commandHandler.Handle(
                    It.Is<AnonymiseConferenceWithHearingIdsCommand>(command => command.HearingIds == request.HearingIds)),
                Times.Once);
        }
        
        [Test]
        public async Task Returns_Not_Found_For_Invalid_HearingIds()
        {
            var request = new AnonymiseConferenceWithHearingIdsRequest{ HearingIds = new List<Guid> {Guid.NewGuid()}} ;
            var exception = new ConferenceNotFoundException(request.HearingIds);

            CommandHandlerMock
                .Setup(commandHandler => commandHandler.Handle(It.IsAny<AnonymiseConferenceWithHearingIdsCommand>()))
                .ThrowsAsync(exception);
            
            var response = await Controller.AnonymiseConferenceWithHearingIds(request) as NotFoundResult;
            
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
