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
        
    }
}
