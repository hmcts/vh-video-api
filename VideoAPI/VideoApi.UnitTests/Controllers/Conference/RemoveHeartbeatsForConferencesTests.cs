using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Video.API.Controllers;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;

namespace VideoApi.UnitTests.Controllers.Conference
{
    public class RemoveHeartbeatsForConferencesTests : ConferenceControllerTestBase
    {
        [Test]
        public async Task Should_successfully_remove_heartbeat_for_conferences_and_return_nocontent()
        {
            var result = await Controller.RemoveHeartbeatsForConferencesAsync();

            var typedResult = (NoContentResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<RemoveHeartbeatsForConferencesCommand>()), Times.Once);
        }
    }
}
