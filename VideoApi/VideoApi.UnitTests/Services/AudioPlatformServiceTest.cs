using System;
using System.Net;
using System.Net.Http;
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
        private readonly Mock<IWowzaHttpClient> _wowzaClient1;
        private readonly Mock<IWowzaHttpClient> _wowzaClientLoadBalancer;
        private readonly WowzaConfiguration _wowzaConfiguration;

        private readonly AudioPlatformService _audioPlatformService;

        public AudioPlatformServiceTest()
        {
            _wowzaClient1 = new Mock<IWowzaHttpClient>();
            _wowzaClientLoadBalancer = new Mock<IWowzaHttpClient>();
            _wowzaClientLoadBalancer.SetupProperty(e => e.IsLoadBalancer, true);
            _wowzaConfiguration = new WowzaConfiguration {StreamingEndpoint = "http://streamIt.com/", ApplicationName = "vh-recording-app"};
            var logger = new Mock<ILogger<AudioPlatformService>>();
            
            _audioPlatformService = new AudioPlatformService(new []{_wowzaClient1.Object, _wowzaClientLoadBalancer.Object}, _wowzaConfiguration, logger.Object);
        }

        [Test]
        public async Task GetAudioApplicationInfoAsync_Returns_Null_When_AudioPlatformException_Thrown()
        {
            _wowzaClient1
                .Setup(x => x.GetApplicationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.InternalServerError));

            var result = await _audioPlatformService.GetAudioApplicationInfoAsync();

            result.Should().BeNull();
        }

        [Test]
        public async Task GetAudioApplicationInfoAsync_Returns_Null_When_AudioPlatformException_Thrown_which_is_NotFound()
        {
            _wowzaClient1
                .Setup(x => x.GetApplicationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.NotFound));

            var result = await _audioPlatformService.GetAudioApplicationInfoAsync();

            result.Should().BeNull();
        }

        [Test]
        public async Task GetAudioApplicationInfoAsync_Returns_Result_Even_When_One_Node_Returns_Null_When_AudioPlatformException_Thrown_which_is_NotFound()
        {
            _wowzaClient1
                .Setup(x => x.GetApplicationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.NotFound));

            _wowzaClientLoadBalancer
                .Setup(x => x.GetApplicationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new WowzaGetApplicationResponse());

            var result = await _audioPlatformService.GetAudioApplicationInfoAsync();

            result.Should().NotBeNull();
        }
        
        [Test]
        public async Task GetAudioApplicationInfoAsync_Returns_Response()
        {
            _wowzaClient1
                .Setup(x => x.GetApplicationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new WowzaGetApplicationResponse());

            var result = await _audioPlatformService.GetAudioApplicationInfoAsync(It.IsAny<Guid>());

            result.Should().NotBeNull();
        }
        
        [Test]
        public async Task DeleteAudioApplicationAsync_Returns_False_AudioPlatformServiceResponse_When_AudioPlatformException_Thrown()
        {
            _wowzaClient1
                .Setup(x => x.DeleteApplicationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.InternalServerError));

            var result = await _audioPlatformService.DeleteAudioApplicationAsync(It.IsAny<Guid>());

            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("SomeError");
            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Test]
        public async Task DeleteAudioApplicationAsync_Returns_False_Even_When_One_Node_Success_AudioPlatformServiceResponse_When_AudioPlatformException_Thrown()
        {
            _wowzaClient1
                .Setup(x => x.DeleteApplicationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _wowzaClientLoadBalancer
                .Setup(x => x.DeleteApplicationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.InternalServerError));

            var result = await _audioPlatformService.DeleteAudioApplicationAsync(It.IsAny<Guid>());

            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("SomeError");
            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
        
        [Test]
        public async Task DeleteAudioApplicationAsync_Returns_True_AudioPlatformServiceResponse()
        {
            _wowzaClient1
                .Setup(x => x.DeleteApplicationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            _wowzaClientLoadBalancer
                .Setup(x => x.DeleteApplicationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            var result = await _audioPlatformService.DeleteAudioApplicationAsync(It.IsAny<Guid>());

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }
        
        [Test]
        public async Task GetAudioStreamMonitoringInfoAsync_Returns_Null_When_AudioPlatformException_Thrown()
        {
            _wowzaClient1
                .Setup(x => x.MonitoringStreamRecorderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.InternalServerError));

            var result = await _audioPlatformService.GetAudioStreamMonitoringInfoAsync(It.IsAny<Guid>());

            result.Should().BeNull();
        }
        
        [Test]
        public async Task GetAudioStreamMonitoringInfoAsync_Returns_Response()
        {
            _wowzaClient1
                .Setup(x => x.MonitoringStreamRecorderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new WowzaMonitorStreamResponse());

            var result = await _audioPlatformService.GetAudioStreamMonitoringInfoAsync(It.IsAny<Guid>());

            result.Should().NotBeNull();
        }

        [Test]
        public async Task GetAudioStreamMonitoringInfoAsync_Returns_Response__Even_When_One_Node_Errors()
        {
            _wowzaClient1
                .Setup(x => x.MonitoringStreamRecorderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.InternalServerError));

            _wowzaClient1
                .Setup(x => x.MonitoringStreamRecorderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new WowzaMonitorStreamResponse());

            var result = await _audioPlatformService.GetAudioStreamMonitoringInfoAsync(It.IsAny<Guid>());

            result.Should().NotBeNull();
        }
        
        [Test]
        public async Task GetAudioStreamInfoAsync_Returns_Null_When_AudioPlatformException_Thrown()
        {
            _wowzaClient1
                .Setup(x => x.GetStreamRecorderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.InternalServerError));

            var result = await _audioPlatformService.GetAudioStreamInfoAsync(It.IsAny<Guid>());

            result.Should().BeNull();
        }
        
        [Test]
        public async Task GetAudioStreamInfoAsync_Returns_Response()
        {
            _wowzaClient1
                .Setup(x => x.GetStreamRecorderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new WowzaGetStreamRecorderResponse());

            var result = await _audioPlatformService.GetAudioStreamInfoAsync(It.IsAny<Guid>());

            result.Should().NotBeNull();
        }

        [Test]
        public async Task GetAudioStreamInfoAsync_Returns_Response_Using_Two_Nodes()
        {
            _wowzaClient1
                .Setup(x => x.GetStreamRecorderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),It.IsAny<string>()))
                .ReturnsAsync(new WowzaGetStreamRecorderResponse());

            _wowzaClientLoadBalancer
                .Setup(x => x.GetStreamRecorderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new WowzaGetStreamRecorderResponse());

            var result = await _audioPlatformService.GetAudioStreamInfoAsync(It.IsAny<Guid>());

            result.Should().NotBeNull();
        }

        [Test]
        public async Task GetAudioStreamInfoAsync_Returns_Response_Even_When_One_Node_Errors()
        {
            _wowzaClient1
                .Setup(x => x.GetStreamRecorderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new WowzaGetStreamRecorderResponse());

            _wowzaClientLoadBalancer
                .Setup(x => x.GetStreamRecorderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.InternalServerError));

            var result = await _audioPlatformService.GetAudioStreamInfoAsync(It.IsAny<Guid>());

            result.Should().NotBeNull();
        }
        
        [Test]
        public async Task DeleteAudioStreamAsync_Returns_False_AudioPlatformServiceResponse_When_AudioPlatformException_Thrown()
        {
            _wowzaClient1
                .Setup(x => x.StopStreamRecorderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.InternalServerError));

            var result = await _audioPlatformService.DeleteAudioStreamAsync(It.IsAny<Guid>());

            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("SomeError");
            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Test]
        public async Task DeleteAudioStreamAsync_Returns_False_Even_When_One_Node_Success_AudioPlatformServiceResponse_When_AudioPlatformException_Thrown()
        {
            _wowzaClient1
                .Setup(x => x.StopStreamRecorderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.InternalServerError));

            _wowzaClientLoadBalancer
                .Setup(x => x.StopStreamRecorderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var result = await _audioPlatformService.DeleteAudioStreamAsync(It.IsAny<Guid>());

            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("SomeError");
            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
        
        [Test]
        public async Task DeleteAudioStreamAsync_Returns_True_AudioPlatformServiceResponse()
        {
            _wowzaClient1
                .Setup(x => x.StopStreamRecorderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            _wowzaClientLoadBalancer
                .Setup(x => x.StopStreamRecorderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            var result = await _audioPlatformService.DeleteAudioStreamAsync(It.IsAny<Guid>());

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }

        [Test]
        public async Task GetApplicationDiagnosticsAsync_Returns_False_When_AudioPlatformException_Thrown()
        {
            _wowzaClientLoadBalancer
                .Setup(x => x.GetDiagnosticsAsync(It.IsAny<string>()))
                .ThrowsAsync(new AudioPlatformException("SomeError", HttpStatusCode.InternalServerError));

            var result = await _audioPlatformService.GetDiagnosticsAsync();

            result.Should().BeFalse();
        }

        [Test]
        public async Task GetApplicationDiagnosticsAsync_Returns_Response()
        {
            _wowzaClientLoadBalancer
                .Setup(x => x.GetDiagnosticsAsync(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
            
            var result = await _audioPlatformService.GetDiagnosticsAsync();

            result.Should().BeTrue();
        }
        
        
        [Test]
        public void GetAudioIngestUrl_returns_expected_url()
        {
            var hearingId = Guid.NewGuid().ToString();
            var url = _audioPlatformService.GetAudioIngestUrl(hearingId);
            url.Should().Contain(hearingId);
            url.Should().Contain(_wowzaConfiguration.ApplicationName);
            url.Should().Contain(_wowzaConfiguration.StreamingEndpoint);
        }
    }
}
