using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using System.Threading.Tasks;
using VideoApi.DAL.Commands;

namespace VideoApi.UnitTests.Controllers.Conference
{
    public class RemoveHeartbeatsForConferencesTests : ConferenceControllerTestBase
    {
        protected Mock<IBackgroundWorkerQueue> BackgroundWorkerQueueMock;
        
        [Test]
        public async Task Should_successfully_remove_heartbeat_for_conferences_and_return_nocontent()
        {
            BackgroundWorkerQueueMock = Mocker.Mock<IBackgroundWorkerQueue>();
            var result = await Controller.RemoveHeartbeatsForConferencesAsync();

            var typedResult = (NoContentResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
            BackgroundWorkerQueueMock.Verify(c => c.QueueBackgroundWorkItem(It.IsAny<RemoveHeartbeatsForConferencesCommand>()), Times.Once);
        }
    }
}
