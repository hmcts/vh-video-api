using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using Video.API.Controllers;
using Video.API.Factories;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Services.Contracts;
using VideoApi.Services.Responses;

namespace VideoApi.UnitTests.Controllers.AudioRecording
{
    [TestFixture]
    public class AudioRecordingControllerTest
    {
        private Mock<IAudioPlatformService> _audioPlatformService;
        private Mock<IAzureStorageServiceFactory> _storageServiceFactory;
        private Mock<IAzureStorageService> _storageService;
        private Mock<IQueryHandler> _queryHandler;
        private VideoApi.Domain.Conference _testConference;

        private AudioRecordingController _controller;
        
        [SetUp]
        public void Setup()
        {
            _queryHandler = new Mock<IQueryHandler>();
            _audioPlatformService = new Mock<IAudioPlatformService>();
            _storageServiceFactory = new Mock<IAzureStorageServiceFactory>();
            _storageService = new Mock<IAzureStorageService>();

            _controller = new AudioRecordingController
            (
               _storageServiceFactory.Object,  _audioPlatformService.Object,
                new Mock<ILogger<AudioRecordingController>>().Object, _queryHandler.Object
            );
            
            
            _testConference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Claimant", null, null, RoomType.ConsultationRoom1)
                .WithParticipant(UserRole.Representative, "Claimant")
                .WithParticipant(UserRole.Individual, "Defendant")
                .WithParticipant(UserRole.Representative, "Defendant")
                .Build();

            _queryHandler
                .Setup(x =>
                    x.Handle<GetConferenceByHearingRefIdQuery, VideoApi.Domain.Conference>(
                        It.IsAny<GetConferenceByHearingRefIdQuery>()))
                .ReturnsAsync(_testConference);
        }

        [Test]
        public async Task GetAudioApplicationAsync_Returns_NotFound()
        {
            _audioPlatformService
                .Setup(x => x.GetAudioApplicationInfoAsync(It.IsAny<Guid>()))
                .ReturnsAsync((WowzaGetApplicationResponse) null);
            
            var result = await _controller.GetAudioApplicationAsync(It.IsAny<Guid>()) as NotFoundResult;
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
            _storageServiceFactory.Setup(x => x.Create(AzureStorageServiceType.Vh)).Returns(_storageService.Object);
            _storageService.Setup(x => x.FileExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
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
            _storageServiceFactory.Setup(x => x.Create(AzureStorageServiceType.Vh)).Returns(_storageService.Object);
            _storageService.Setup(x => x.FileExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

            _audioPlatformService
                .Setup(x => x.DeleteAudioApplicationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new AudioPlatformServiceResponse(true));
            
            var result = await _controller.DeleteAudioApplicationAsync(It.IsAny<Guid>()) as NoContentResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }

        [Test]
        public async Task Should_not_delete_audio_application_if_audio_file_not_exists_returns_notFound()
        {
            var conferenceType = typeof(VideoApi.Domain.Conference);
            conferenceType.GetProperty(nameof(_testConference.ActualStartTime))
                ?.SetValue(_testConference, DateTime.UtcNow.AddHours(-1));
            _queryHandler
                .Setup(x =>
                    x.Handle<GetConferenceByHearingRefIdQuery, VideoApi.Domain.Conference>(
                        It.IsAny<GetConferenceByHearingRefIdQuery>()))
                .ReturnsAsync(_testConference);

            _storageServiceFactory.Setup(x => x.Create(AzureStorageServiceType.Vh)).Returns(_storageService.Object);
            _storageService.Setup(x => x.FileExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _audioPlatformService.Reset();
            var result = await _controller.DeleteAudioApplicationAsync(It.IsAny<Guid>()) as NotFoundResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            _audioPlatformService.Verify(x => x.DeleteAudioApplicationAsync(It.IsAny<Guid>()), Times.Never);
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

        [Test]
        public async Task GetAudioRecordingLinkAsync_return_notfound()
        {
            var conferenceType = typeof(VideoApi.Domain.Conference);
            conferenceType.GetProperty(nameof(_testConference.ActualStartTime))
                ?.SetValue(_testConference, DateTime.UtcNow.AddHours(-1));
            _queryHandler
                .Setup(x =>
                    x.Handle<GetConferenceByHearingRefIdQuery, VideoApi.Domain.Conference>(
                        It.IsAny<GetConferenceByHearingRefIdQuery>()))
                .ReturnsAsync(_testConference);

            _storageServiceFactory.Setup(x => x.Create(AzureStorageServiceType.Vh)).Returns(_storageService.Object);
            _storageService.Setup(x => x.FileExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            
            var result = await _controller.GetAudioRecordingLinkAsync(It.IsAny<Guid>()) as NotFoundResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }
        
        [Test]
        public async Task GetAudioRecordingLinkAsync_returns_audio_file_link()
        {
            _storageServiceFactory.Setup(x => x.Create(AzureStorageServiceType.Vh)).Returns(_storageService.Object);
            _storageService.Setup(x => x.FileExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            _storageService
                .Setup(x => x.CreateSharedAccessSignature(It.IsAny<string>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync("fileLink");

            var hearingId = Guid.NewGuid();
            var filePath = $"{hearingId}.mp4";
            var result = await _controller.GetAudioRecordingLinkAsync(hearingId) as OkObjectResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            var item = result.Value.As<AudioRecordingResponse>();
            item.Should().NotBeNull();
            item.AudioFileLink.Should().NotBeNullOrEmpty();
            item.AudioFileLink.Should().Be("fileLink");
            _storageService.Verify(c=>c.CreateSharedAccessSignature(filePath, It.IsAny<TimeSpan>()));
        }
    }
}
