using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;

namespace VideoApi.UnitTests.Controllers.Conference
{
    public class RemoveHeartbeatsForConferencesTests : ConferenceControllerTestBase
    {
        protected Mock<BackgroundWorkerQueue> BackgroundWorkerQueueMock;
        
        [Test]
        public async Task Should_successfully_remove_heartbeat_for_conferences_and_return_nocontent()
        {
            BackgroundWorkerQueueMock = Mocker.Mock<BackgroundWorkerQueue>();
            var result = await Controller.RemoveHeartbeatsForConferencesAsync();

            var typedResult = (NoContentResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
            BackgroundWorkerQueueMock.Verify(c => c.QueueBackgroundWorkItem(It.IsAny<Func<CancellationToken,Task>>()), Times.Once);
        }
    }
}
