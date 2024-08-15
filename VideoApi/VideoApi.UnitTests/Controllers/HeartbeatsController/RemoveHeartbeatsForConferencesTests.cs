using System.Net;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using VideoApi.DAL.Commands;

namespace VideoApi.UnitTests.Controllers.HeartbeatsController;

public class RemoveHeartbeatsForConferencesTests
{
    private Mock<IBackgroundWorkerQueue> _backgroundWorkerQueueMock;
    private Mock<ILogger<VideoApi.Controllers.HeartbeatsController>> _mockLogger;
    
    public RemoveHeartbeatsForConferencesTests()
    {
        var mocker = AutoMock.GetLoose();
        _backgroundWorkerQueueMock = mocker.Mock<IBackgroundWorkerQueue>();
        _mockLogger = mocker.Mock<ILogger<VideoApi.Controllers.HeartbeatsController>>();
    }
    
    [Test]
    public async Task Should_successfully_remove_heartbeat_for_conferences_and_return_nocontent()
    {
        var heartbeatsController =
            new VideoApi.Controllers.HeartbeatsController(_mockLogger.Object, _backgroundWorkerQueueMock.Object);
        var result = await heartbeatsController.RemoveHeartbeatsForConferencesAsync();
        
        var typedResult = (NoContentResult)result;
        typedResult.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        _backgroundWorkerQueueMock.Verify(
            c => c.QueueBackgroundWorkItem(It.IsAny<RemoveHeartbeatsForConferencesCommand>()), Times.Once);
    }
}
