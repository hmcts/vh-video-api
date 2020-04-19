using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Video.API.Controllers;
using VideoApi.Contract.Responses;
using VideoApi.Services.Contracts;
using VideoApi.Services.Responses;

namespace VideoApi.UnitTests.Controllers.AudioRecording
{
    [TestFixture]
    public class AudioRecordingControllerTest
    {
        private readonly Mock<IAudioPlatformService> _audioPlatformService;
        private readonly Mock<ILogger<AudioRecordingController>> _logger;
        
        private readonly AudioRecordingController _controller;
        
        public AudioRecordingControllerTest()
        {
            _audioPlatformService = new Mock<IAudioPlatformService>();
            _logger = new Mock<ILogger<AudioRecordingController>>();
            
            _controller = new AudioRecordingController(_audioPlatformService.Object, _logger.Object);    
        }

        [Test]
        public async Task GetAudioApplicationAsync_Returns_NotFound()
        {
            _audioPlatformService
                .Setup(x => x.GetAudioApplicationInfoAsync(It.IsAny<Guid>()))
                .ReturnsAsync((WowzaGetApplicationResponse) null);
            
            var result = await _controller.GetAudioStreamInfoAsync(It.IsAny<Guid>()) as NotFoundResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }
        
        [Test]
        public async Task GetAudioApplicationAsync_Returns_AudioApplicationInfoResponse()
        {
            var wowzaResponse = new WowzaGetApplicationResponse
            {
                Description = "Description",
                Name = "Name",
                ServerName = "ServerName",
                StreamConfig = new Streamconfig
                {
                    KeyDir = "KeyDir",
                    ServerName = "ServerName",
                    StorageDir = "StorageDir",
                    StreamType = "StreamType",
                    StorageDirExists = true
                }
            };
            
            _audioPlatformService
                .Setup(x => x.GetAudioApplicationInfoAsync(It.IsAny<Guid>()))
                .ReturnsAsync(wowzaResponse);
            
            var result = await _controller.GetAudioApplicationAsync(It.IsAny<Guid>()) as OkObjectResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            var response = result.Value as AudioApplicationInfoResponse;
            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(wowzaResponse, options => options.ExcludingMissingMembers());
        }
        
        [Test]
        public async Task CreateAudioApplicationAsync_Returns_NotFound()
        {
            _audioPlatformService
                .Setup(x => x.CreateAudioApplicationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new AudioPlatformServiceResponse(false)
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "Not Found"
                });
            
            var result = await _controller.CreateAudioApplicationAsync(It.IsAny<Guid>()) as ObjectResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            result.Value.Should().Be("Not Found");
        }
        
        [Test]
        public async Task CreateAudioApplicationAsync_Returns_Conflict()
        {
            _audioPlatformService
                .Setup(x => x.CreateAudioApplicationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new AudioPlatformServiceResponse(false)
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Message = "Conflict"
                });
            
            var result = await _controller.CreateAudioApplicationAsync(It.IsAny<Guid>()) as ObjectResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status409Conflict);
            result.Value.Should().Be("Conflict");
        }
        
        [Test]
        public async Task CreateAudioApplicationAsync_Returns_Ok()
        {
            _audioPlatformService
                .Setup(x => x.CreateAudioApplicationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new AudioPlatformServiceResponse(true));
            
            var result = await _controller.CreateAudioApplicationAsync(It.IsAny<Guid>()) as OkResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
        }
        
        [Test]
        public async Task CreateAudioApplicationWithStreamAsync_Returns_Conflict()
        {
            _audioPlatformService
                .Setup(x => x.CreateAudioApplicationWithStreamAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new AudioPlatformServiceResponse(false)
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Message = "Conflict"
                });
            
            var result = await _controller.CreateAudioApplicationWithStreamAsync(It.IsAny<Guid>()) as ObjectResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status409Conflict);
            result.Value.Should().Be("Conflict");
        }
        
        [Test]
        public async Task CreateAudioApplicationWithStreamAsync_Returns_IngestUrl()
        {
            _audioPlatformService
                .Setup(x => x.CreateAudioApplicationWithStreamAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new AudioPlatformServiceResponse(true) { IngestUrl = "IngestUrl"});
            
            var result = await _controller.CreateAudioApplicationWithStreamAsync(It.IsAny<Guid>()) as OkObjectResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            result.Value.Should().Be("IngestUrl");
        }
        
        [Test]
        public async Task DeleteAudioApplicationAsync_Returns_Conflict()
        {
            _audioPlatformService
                .Setup(x => x.DeleteAudioApplicationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new AudioPlatformServiceResponse(false)
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Message = "Conflict"
                });
            
            var result = await _controller.DeleteAudioApplicationAsync(It.IsAny<Guid>()) as ObjectResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status409Conflict);
            result.Value.Should().Be("Conflict");
        }
        
        [Test]
        public async Task DeleteAudioApplicationAsync_Returns_NoContent()
        {
            _audioPlatformService
                .Setup(x => x.DeleteAudioApplicationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new AudioPlatformServiceResponse(true));
            
            var result = await _controller.DeleteAudioApplicationAsync(It.IsAny<Guid>()) as NoContentResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }
        
        [Test]
        public async Task GetAudioStreamInfoAsync_Returns_NotFound()
        {
            _audioPlatformService
                .Setup(x => x.GetAudioStreamInfoAsync(It.IsAny<Guid>()))
                .ReturnsAsync((WowzaGetStreamRecorderResponse) null);
            
            var result = await _controller.GetAudioStreamInfoAsync(It.IsAny<Guid>()) as NotFoundResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }
        
        [Test]
        public async Task GetAudioStreamInfoAsync_Returns_AudioStreamInfoResponse()
        {
            var wowzaResponse = new WowzaGetStreamRecorderResponse
            {
                Option = "Option",
                ApplicationName = "ApplicationName",
                BaseFile = "BaseFile",
                CurrentDuration = 1,
                CurrentFile = "CurrentFile",
                CurrentSize = 1,
                FileFormat = "FileFormat",
                InstanceName = "InstanceName",
                OutputPath = "OutputPath",
                RecorderName = "RecorderName",
                RecorderState = "RecorderState",
                SegmentDuration = 1,
                ServerName = "ServerName",
                RecorderErrorString = "RecorderErrorString",
                RecordingStartTime = "RecordingStartTime"
            };
            
            _audioPlatformService
                .Setup(x => x.GetAudioStreamInfoAsync(It.IsAny<Guid>()))
                .ReturnsAsync(wowzaResponse);
            
            var result = await _controller.GetAudioStreamInfoAsync(It.IsAny<Guid>()) as OkObjectResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            var response = result.Value as AudioStreamInfoResponse;
            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(wowzaResponse, options => options.ExcludingMissingMembers());
        }
        
        [Test]
        public async Task GetAudioStreamMonitoringInfoAsync_Returns_NotFound()
        {
            _audioPlatformService
                .Setup(x => x.GetAudioStreamMonitoringInfoAsync(It.IsAny<Guid>()))
                .ReturnsAsync((WowzaMonitorStreamResponse) null);
            
            var result = await _controller.GetAudioStreamMonitoringInfoAsync(It.IsAny<Guid>()) as NotFoundResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }
        
        [Test]
        public async Task GetAudioStreamMonitoringInfoAsync_Returns_AudioStreamMonitoringInfo()
        {
            var wowzaResponse = new WowzaMonitorStreamResponse
            {
                Name = "Name",
                Uptime = 1,
                ApplicationInstance = "ApplicationInstance",
                BytesIn = 1,
                ServerName = "ServerName",
                BytesInRate = 1
            };
            
            _audioPlatformService
                .Setup(x => x.GetAudioStreamMonitoringInfoAsync(It.IsAny<Guid>()))
                .ReturnsAsync(wowzaResponse);
            
            var result = await _controller.GetAudioStreamMonitoringInfoAsync(It.IsAny<Guid>()) as OkObjectResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            var response = result.Value as AudioStreamMonitoringInfo;
            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(wowzaResponse, options => options.ExcludingMissingMembers());
        }
        
        [Test]
        public async Task CreateAudioStreamAsync_Returns_Conflict()
        {
            _audioPlatformService
                .Setup(x => x.CreateAudioStreamAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new AudioPlatformServiceResponse(false)
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Message = "Conflict"
                });
            
            var result = await _controller.CreateAudioStreamAsync(It.IsAny<Guid>()) as ObjectResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status409Conflict);
            result.Value.Should().Be("Conflict");
        }
        
        [Test]
        public async Task CreateAudioStreamAsync_Returns_IngestUrl()
        {
            _audioPlatformService
                .Setup(x => x.CreateAudioStreamAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new AudioPlatformServiceResponse(true) { IngestUrl = "IngestUrl"});
            
            var result = await _controller.CreateAudioStreamAsync(It.IsAny<Guid>()) as OkObjectResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            result.Value.Should().Be("IngestUrl");
        }
        
        [Test]
        public async Task DeleteAudioStreamAsync_Returns_Conflict()
        {
            _audioPlatformService
                .Setup(x => x.DeleteAudioStreamAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new AudioPlatformServiceResponse(false)
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Message = "Conflict"
                });
            
            var result = await _controller.DeleteAudioStreamAsync(It.IsAny<Guid>()) as ObjectResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status409Conflict);
            result.Value.Should().Be("Conflict");
        }
        
        [Test]
        public async Task DeleteAudioStreamAsync_Returns_NoContent()
        {
            _audioPlatformService
                .Setup(x => x.DeleteAudioStreamAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new AudioPlatformServiceResponse(true));
            
            var result = await _controller.DeleteAudioStreamAsync(It.IsAny<Guid>()) as NoContentResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }
    }
}
