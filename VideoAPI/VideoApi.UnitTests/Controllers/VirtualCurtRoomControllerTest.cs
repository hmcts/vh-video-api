using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Video.API.Controllers;
using VideoApi.Services;
using VideoApi.Services.Kinly;

namespace VideoApi.UnitTests.Controllers
{

    [TestFixture]
    public class VirtualCurtRoomControllerTest
    {
        private readonly Mock<IVideoPlatformService> _videoPlatformService;
        private readonly Mock<ILogger<ConferenceController>> _logger;
        private readonly VirtualCurtRoomController _virtualCurtRoomController;

        public VirtualCurtRoomControllerTest()
        {
            _videoPlatformService = new Mock<IVideoPlatformService>();
            _logger = new Mock<ILogger<ConferenceController>>();

            _virtualCurtRoomController = new VirtualCurtRoomController
            (
                 _videoPlatformService.Object,
                _logger.Object
            );
        }

        [Test]
        public async Task RemoveVirtualCourtRoom_Returns_NoContent_Success()
        {
            var hearingRefId = Guid.NewGuid();

            _videoPlatformService.Setup(x => x.DeleteVirtualCourtRoomAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

            var result = await _virtualCurtRoomController.RemoveVirtualCourtRoom(hearingRefId);

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<NoContentResult>();
        }

        [Test]
        public async Task RemoveVirtualCourtRoom_Returns_Error_BadRequest()
        {
            var hearingRefId = Guid.NewGuid();

            _videoPlatformService.Setup(x => x.DeleteVirtualCourtRoomAsync(It.IsAny<string>())).Throws(new KinlyApiException("Error", 500, "", new Dictionary<string, IEnumerable<string>>(), new Exception()));

            var result = await _virtualCurtRoomController.RemoveVirtualCourtRoom(hearingRefId);

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [Test]
        public async Task RemoveVirtualCourtRoom_Returns_NotFound()
        {
            var hearingRefId = Guid.NewGuid();

            _videoPlatformService.Setup(x => x.DeleteVirtualCourtRoomAsync(It.IsAny<string>())).Throws(new KinlyApiException("Error", 404, "", new Dictionary<string, IEnumerable<string>>(), new Exception()));

            var result = await _virtualCurtRoomController.RemoveVirtualCourtRoom(hearingRefId);

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<NotFoundResult>();
        }
    }
}
