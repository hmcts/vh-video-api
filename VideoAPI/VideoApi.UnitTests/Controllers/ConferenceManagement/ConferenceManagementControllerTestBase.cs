using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Video.API.Controllers;
using VideoApi.Services.Contracts;

namespace VideoApi.UnitTests.Controllers.ConferenceManagement
{
    public class ConferenceManagementControllerTestBase
    {
        protected ConferenceManagementController Controller;
        protected Mock<ILogger<ConferenceManagementController>> MockLogger;
        protected Mock<IVideoPlatformService> VideoPlatformServiceMock;
        
        [SetUp]
        public void Setup()
        {
            MockLogger = new Mock<ILogger<ConferenceManagementController>>();
            VideoPlatformServiceMock = new Mock<IVideoPlatformService>();
            
            Controller = new ConferenceManagementController(VideoPlatformServiceMock.Object, MockLogger.Object);
        }
    }
}
