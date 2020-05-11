using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using VideoApi.Common.Configuration;
using VideoApi.Services;
using VideoApi.Services.Contracts;
using VideoApi.Services.Exceptions;
using VideoApi.Services.Responses;

namespace VideoApi.UnitTests.Services
{
    [TestFixture]
    public class AudioPlatformServiceTest
    {
        private readonly Mock<IWowzaHttpClient> _wowzaClient;
        private readonly WowzaConfiguration _wowzaConfiguration;

        private readonly AudioPlatformService _audioPlatformService;

        public AudioPlatformServiceTest()
        {
            _wowzaClient = new Mock<IWowzaHttpClient>();
            _wowzaConfiguration = new WowzaConfiguration {StreamingEndpoint = "http://streamIt.com/"};
            var logger = new Mock<ILogger<AudioPlatformService>>();
            
            _audioPlatformService = new AudioPlatformService(_wowzaClient.Object, _wowzaConfiguration, logger.Object);
        }

        [Test]
        public async Task GetAudioApplicationInfoAsync_Returns_Null_When_AudioPlatformException_Thrown()
        {
            _wowzaClient
                .Setup(x => x.GetApplicationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.InternalServerError));

            var result = await _audioPlatformService.GetAudioApplicationInfoAsync(It.IsAny<Guid>());

            result.Should().BeNull();
        }
        
        [Test]
        public async Task GetAudioApplicationInfoAsync_Returns_Null_When_AudioPlatformException_Thrown_which_is_NotFound()
        {
            _wowzaClient
                .Setup(x => x.GetApplicationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.NotFound));

            var result = await _audioPlatformService.GetAudioApplicationInfoAsync(It.IsAny<Guid>());

            result.Should().BeNull();
        }
        
        [Test]
        public async Task GetAudioApplicationInfoAsync_Returns_Response()
        {
            _wowzaClient
                .Setup(x => x.GetApplicationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new WowzaGetApplicationResponse());

            var result = await _audioPlatformService.GetAudioApplicationInfoAsync(It.IsAny<Guid>());

            result.Should().NotBeNull();
        }
        
        [Test]
        public async Task GetAllAudioApplicationsInfoAsync_Returns_Null_When_AudioPlatformException_Thrown_which_is_NotFound()
        {
            _wowzaClient
                .Setup(x => x.GetApplicationsAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.NotFound));

            var result = await _audioPlatformService.GetAllAudioApplicationsInfoAsync();

            result.Should().BeNull();
        }
        
        [Test]
        public async Task GetAllAudioApplicationsInfoAsync_Returns_Null_When_AudioPlatformException_Thrown()
        {
            _wowzaClient
                .Setup(x => x.GetApplicationsAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.InternalServerError));

            var result = await _audioPlatformService.GetAllAudioApplicationsInfoAsync();

            result.Should().BeNull();
        }
        
        [Test]
        public async Task GetAllAudioApplicationsInfoAsync_Returns_Response()
        {
            _wowzaClient
                .Setup(x => x.GetApplicationsAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new WowzaGetApplicationsResponse());

            var result = await _audioPlatformService.GetAllAudioApplicationsInfoAsync();

            result.Should().NotBeNull();
        }
        
        [Test]
        public async Task CreateAudioApplicationAsync_Returns_False_AudioPlatformServiceResponse_When_AudioPlatformException_Thrown()
        {
            _wowzaClient
                .Setup(x => x.CreateApplicationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.InternalServerError));

            var result = await _audioPlatformService.CreateAudioApplicationAsync(It.IsAny<Guid>());

            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("SomeError");
            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
        
        [Test]
        public async Task CreateAudioApplicationAsync_Returns_Null_When_AudioPlatformException_Thrown_which_is_NotFound()
        {
            _wowzaClient
                .Setup(x => x.CreateApplicationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.NotFound));

            var result = await _audioPlatformService.CreateAudioApplicationAsync(It.IsAny<Guid>());

            result.Should().BeNull();
        }

        [Test]
        public async Task CreateAudioApplicationAsync_Returns_False_AudioPlatformServiceResponse_When_AudioPlatformException_Thrown_On_Update()
        {
            _wowzaClient
                .Setup(x => x.UpdateApplicationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.InternalServerError));

            var result = await _audioPlatformService.CreateAudioApplicationAsync(It.IsAny<Guid>());

            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("SomeError");
            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Test]
        public async Task CreateAudioApplicationAsync_Returns_True_AudioPlatformServiceResponse()
        {
            _wowzaClient
                .Setup(x => x.CreateApplicationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            _wowzaClient
               .Setup(x => x.UpdateApplicationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            var result = await _audioPlatformService.CreateAudioApplicationAsync(It.IsAny<Guid>());

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }
        
        [Test]
        public async Task CreateAudioApplicationWithStreamAsync_Returns_False_AudioPlatformServiceResponse_When_AudioPlatformException_Thrown_With_DefaultIngestUrl()
        {
            _wowzaClient
                .Setup(x => x.CreateApplicationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.InternalServerError));

            var result = await _audioPlatformService.CreateAudioApplicationWithStreamAsync(It.IsAny<Guid>());

            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("SomeError");
            result.IngestUrl.Should().Be(" ");
            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Test]
        public async Task CreateAudioApplicationWithStreamAsync_Returns_False_AudioPlatformServiceResponse_When_AudioPlatformException_Thrown_On_Update_With_DefaultIngestUrl()
        {
            _wowzaClient
                .Setup(x => x.UpdateApplicationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.InternalServerError));

            var result = await _audioPlatformService.CreateAudioApplicationWithStreamAsync(It.IsAny<Guid>());

            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("SomeError");
            result.IngestUrl.Should().Be(" ");
            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
        
        [Test]
        public async Task CreateAudioApplicationWithStreamAsync_Returns_Null_When_AudioPlatformException_which_is_NotFound()
        {
            _wowzaClient
                .Setup(x => x.CreateApplicationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.NotFound));

            var result = await _audioPlatformService.CreateAudioApplicationWithStreamAsync(It.IsAny<Guid>());

            result.Should().BeNull();
        }

        [Test]
        public async Task CreateAudioApplicationWithStreamAsync_Returns_True_AudioPlatformServiceResponse_With_IngestUrl()
        {
            _wowzaClient
                .Setup(x => x.CreateApplicationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            _wowzaClient
              .Setup(x => x.UpdateApplicationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            _wowzaClient
                .Setup(x => x.AddStreamRecorderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            var hearingId = Guid.NewGuid();
            var result = await _audioPlatformService.CreateAudioApplicationWithStreamAsync(hearingId);
        
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.IngestUrl.Should().Be($"{_wowzaConfiguration.StreamingEndpoint}{hearingId}/{hearingId}");
        }
        
        [Test]
        public async Task DeleteAudioApplicationAsync_Returns_False_AudioPlatformServiceResponse_When_AudioPlatformException_Thrown()
        {
            _wowzaClient
                .Setup(x => x.DeleteApplicationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.InternalServerError));

            var result = await _audioPlatformService.DeleteAudioApplicationAsync(It.IsAny<Guid>());

            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("SomeError");
            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
        
        [Test]
        public async Task DeleteAudioApplicationAsync_Returns_Null_When_AudioPlatformException_Thrown_which_is_NotFound()
        {
            _wowzaClient
                .Setup(x => x.DeleteApplicationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.NotFound));

            var result = await _audioPlatformService.DeleteAudioApplicationAsync(It.IsAny<Guid>());

            result.Should().BeNull();
        }
        
        [Test]
        public async Task DeleteAudioApplicationAsync_Returns_True_AudioPlatformServiceResponse()
        {
            _wowzaClient
                .Setup(x => x.DeleteApplicationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            var result = await _audioPlatformService.DeleteAudioApplicationAsync(It.IsAny<Guid>());

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }
        
        [Test]
        public async Task GetAudioStreamMonitoringInfoAsync_Returns_Null_When_AudioPlatformException_Thrown()
        {
            _wowzaClient
                .Setup(x => x.MonitoringStreamRecorderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.InternalServerError));

            var result = await _audioPlatformService.GetAudioStreamMonitoringInfoAsync(It.IsAny<Guid>());

            result.Should().BeNull();
        }
        
        [Test]
        public async Task GetAudioStreamMonitoringInfoAsync_Returns_Null_When_AudioPlatformException_Thrown_which_is_NotFound()
        {
            _wowzaClient
                .Setup(x => x.MonitoringStreamRecorderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.NotFound));

            var result = await _audioPlatformService.GetAudioStreamMonitoringInfoAsync(It.IsAny<Guid>());

            result.Should().BeNull();
        }
        
        [Test]
        public async Task GetAudioStreamMonitoringInfoAsync_Returns_Response()
        {
            _wowzaClient
                .Setup(x => x.MonitoringStreamRecorderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new WowzaMonitorStreamResponse());

            var result = await _audioPlatformService.GetAudioStreamMonitoringInfoAsync(It.IsAny<Guid>());

            result.Should().NotBeNull();
        }
        
        [Test]
        public async Task GetAudioStreamInfoAsync_Returns_Null_When_AudioPlatformException_Thrown()
        {
            _wowzaClient
                .Setup(x => x.GetStreamRecorderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.InternalServerError));

            var result = await _audioPlatformService.GetAudioStreamInfoAsync(It.IsAny<Guid>());

            result.Should().BeNull();
        }
        
        [Test]
        public async Task GetAudioStreamInfoAsync_Returns_Null_When_AudioPlatformException_Thrown_which_is_NotFound()
        {
            _wowzaClient
                .Setup(x => x.GetStreamRecorderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.NotFound));

            var result = await _audioPlatformService.GetAudioStreamInfoAsync(It.IsAny<Guid>());

            result.Should().BeNull();
        }
        
        [Test]
        public async Task GetAudioStreamInfoAsync_Returns_Response()
        {
            _wowzaClient
                .Setup(x => x.GetStreamRecorderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new WowzaGetStreamRecorderResponse());

            var result = await _audioPlatformService.GetAudioStreamInfoAsync(It.IsAny<Guid>());

            result.Should().NotBeNull();
        }
        
        [Test]
        public async Task CreateAudioStreamAsync_Returns_False_AudioPlatformServiceResponse_When_AudioPlatformException_Thrown_With_Default_IngestUrl()
        {
            _wowzaClient
                .Setup(x => x.AddStreamRecorderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.InternalServerError));

            var result = await _audioPlatformService.CreateAudioStreamAsync(It.IsAny<Guid>());

            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("SomeError");
            result.IngestUrl.Should().Be($" ");
            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
        
        [Test]
        public async Task CreateAudioStreamAsync_Returns_Null_When_AudioPlatformException_Thrown_With_Default_IngestUrl_which_is_NotFound()
        {
            _wowzaClient
                .Setup(x => x.AddStreamRecorderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.NotFound));

            var result = await _audioPlatformService.CreateAudioStreamAsync(It.IsAny<Guid>());

            result.Should().BeNull();
        }
        
        [Test]
        public async Task CreateAudioStreamAsync_Returns_True_AudioPlatformServiceResponse_With_IngestUrl()
        {
            _wowzaClient
                .Setup(x => x.AddStreamRecorderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            var hearingId = Guid.NewGuid();
            var result = await _audioPlatformService.CreateAudioStreamAsync(hearingId);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.IngestUrl.Should().Be($"{_wowzaConfiguration.StreamingEndpoint}{hearingId}/{hearingId}");
        }
        
        [Test]
        public async Task DeleteAudioStreamAsync_Returns_False_AudioPlatformServiceResponse_When_AudioPlatformException_Thrown()
        {
            _wowzaClient
                .Setup(x => x.StopStreamRecorderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.InternalServerError));

            var result = await _audioPlatformService.DeleteAudioStreamAsync(It.IsAny<Guid>());

            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("SomeError");
            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
        
        [Test]
        public async Task DeleteAudioStreamAsync_Returns_Null_When_AudioPlatformException_Thrown_which_is_NotFound()
        {
            _wowzaClient
                .Setup(x => x.StopStreamRecorderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.NotFound));

            var result = await _audioPlatformService.DeleteAudioStreamAsync(It.IsAny<Guid>());

            result.Should().BeNull();
        }
        
        [Test]
        public async Task DeleteAudioStreamAsync_Returns_True_AudioPlatformServiceResponse()
        {
            _wowzaClient
                .Setup(x => x.StopStreamRecorderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            var result = await _audioPlatformService.DeleteAudioStreamAsync(It.IsAny<Guid>());

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }
    }
}
