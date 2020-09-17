using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using VideoApi.Services.Clients;
using VideoApi.Services.Contracts;
using VideoApi.Services.Exceptions;

namespace VideoApi.UnitTests.Clients
{
    [TestFixture]
    public class WowzaHttpClientTest
    {
        private IWowzaHttpClient wowzaHttpClient;

        private void SetupWithBaseAddress(Uri baseAddress)
        {
            Mock<ICreateHttpClientFactory> _createHttpClientFactoryMock;
            _createHttpClientFactoryMock = new Mock<ICreateHttpClientFactory>();

            var model = new WowzaClientModel
            {
                HostName = "host",
                HttpClientForNode = new HttpClient(new FakeHttpMessageHandler())
                {
                    BaseAddress = baseAddress
                },
                ServerName = "server"
            };
            _createHttpClientFactoryMock.Setup(x => x.GetHttpClients()).Returns(new List<WowzaClientModel> { model, model });

            wowzaHttpClient = new WowzaHttpClient(_createHttpClientFactoryMock.Object);
        }

        [Test]
        public void CreateApplicationAsync_Throws_AudioPlatformException_On_Http_Failure()
        {
            // Case 1
            SetupWithBaseAddress(new Uri($"http://{nameof(Exception)}.com/"));
            var exception = Assert.ThrowsAsync<Exception>
            (
                () => wowzaHttpClient.CreateApplicationAsync
                (
                    It.IsAny<string>(), It.IsAny<string>()
                )
            );
            exception.Message.Should().Be("Exception thrown");
        }

        [Test]
        public void CreateApplicationAsync_Success()
        {
            // Case 1
            SetupWithBaseAddress(new Uri($"http://{HttpStatusCode.OK}.com/"));

            var result = wowzaHttpClient.CreateApplicationAsync
            (
                It.IsAny<string>(), It.IsAny<string>()
            );

            result.IsCompleted.Should().BeTrue();
            result.IsFaulted.Should().BeFalse();
        }

        [Test]
        public void UpdateApplicationAsync_Throws_AudioPlatformException_On_Http_Failure()
        {
            SetupWithBaseAddress(new Uri($"http://{nameof(Exception)}.com/"));

            // Case 1

            var exception = Assert.ThrowsAsync<Exception>
            (
                () => wowzaHttpClient.UpdateApplicationAsync
                (
                    It.IsAny<string>(), It.IsAny<string>()
                )
            );
            exception.Message.Should().Be("Exception thrown");
        }

        [Test]
        public void UpdateApplicationAsync_Success()
        {
            SetupWithBaseAddress(new Uri($"http://{HttpStatusCode.OK}.com/"));

            // Case 1

            var result = wowzaHttpClient.UpdateApplicationAsync
            (
                It.IsAny<string>(), It.IsAny<string>()
            );

            result.IsCompleted.Should().BeTrue();
            result.IsFaulted.Should().BeFalse();
        }

        [Test]
        public void DeleteApplicationAsync_Throws_AudioPlatformException_On_Http_Failure()
        {
            SetupWithBaseAddress(new Uri($"http://{nameof(Exception)}.com/"));

            // Case 1

            var exception = Assert.ThrowsAsync<Exception>
            (
                () => wowzaHttpClient.DeleteApplicationAsync
                (
                    It.IsAny<string>()
                )
            );
            exception.Message.Should().Be("Exception thrown");
        }

        [Test]
        public void DeleteApplicationAsync_Success()
        {
            SetupWithBaseAddress(new Uri($"http://{HttpStatusCode.OK}.com/"));

            // Case 1

            var result = wowzaHttpClient.DeleteApplicationAsync
            (
                It.IsAny<string>()
            );

            result.IsCompleted.Should().BeTrue();
            result.IsFaulted.Should().BeFalse();
        }

        [Test]
        public void AddStreamRecorderAsync_Throws_AudioPlatformException_On_Http_Failure()
        {
            SetupWithBaseAddress(new Uri($"http://{nameof(Exception)}.com/"));

            // Case 1

            var client = wowzaHttpClient;
            var exception = Assert.ThrowsAsync<Exception>
            (
                () => client.AddStreamRecorderAsync
                (
                    It.IsAny<string>()
                )
            );
            exception.Message.Should().Be("Exception thrown");

            SetupWithBaseAddress(new Uri($"http://{HttpStatusCode.BadRequest}.com/"));

            // Case 2

            exception = Assert.ThrowsAsync<AudioPlatformException>
            (
                () => wowzaHttpClient.AddStreamRecorderAsync
                (
                    It.IsAny<string>()
                )
            );
            exception.Message.Should().Be("Bad request");
        }

        [Test]
        public void AddStreamRecorderAsync_Success()
        {
            SetupWithBaseAddress(new Uri($"http://{HttpStatusCode.OK}.com/"));

            // Case 1
            var result = wowzaHttpClient.AddStreamRecorderAsync
            (
                It.IsAny<string>()
            );

            result.IsCompleted.Should().BeTrue();
            result.IsFaulted.Should().BeFalse();
        }

        [Test]
        public void MonitoringStreamRecorderAsync_Throws_AudioPlatformException_On_Http_Failure()
        {
            SetupWithBaseAddress(new Uri($"http://{nameof(Exception)}.com/"));

            // Case 1

            var exception = Assert.ThrowsAsync<Exception>
            (
                () => wowzaHttpClient.MonitoringStreamRecorderAsync
                (
                    It.IsAny<string>()
                )
            );
            exception.Message.Should().Be("Exception thrown");
        }

        [Test]
        public async Task MonitoringStreamRecorderAsync_Success()
        {
            SetupWithBaseAddress(new Uri($"http://{HttpStatusCode.OK}.com/"));

            // Case 1

            var result = await wowzaHttpClient.MonitoringStreamRecorderAsync
            (
                It.IsAny<string>()
            );

            result.Should().NotBeNull();
        }

        [Test]
        public void GetApplicationAsync_Throws_AudioPlatformException_On_Http_Failure()
        {
            SetupWithBaseAddress(new Uri($"http://{nameof(Exception)}.com/"));

            // Case 1

            var exception = Assert.ThrowsAsync<Exception>
            (
                () => wowzaHttpClient.GetApplicationAsync
                (
                    It.IsAny<string>()
                )
            );
            exception.Message.Should().Be("Exception thrown");
        }

        [Test]
        public async Task GetApplicationAsync_Success()
        {
            SetupWithBaseAddress(new Uri($"http://{HttpStatusCode.OK}.com/"));

            // Case 1

            var result = await wowzaHttpClient.GetApplicationAsync
            (
                It.IsAny<string>()
            );

            result.Should().NotBeNull();
        }

        [Test]
        public void GetStreamRecorderAsync_Throws_AudioPlatformException_On_Http_Failure()
        {
            SetupWithBaseAddress(new Uri($"http://{nameof(Exception)}.com/"));

            // Case 1

            var exception = Assert.ThrowsAsync<Exception>
            (
                () => wowzaHttpClient.GetStreamRecorderAsync
                (
                    It.IsAny<string>()
                )
            );
            exception.Message.Should().Be("Exception thrown");
        }

        [Test]
        public async Task GetStreamRecorderAsync_Success()
        {
            SetupWithBaseAddress(new Uri($"http://{HttpStatusCode.OK}.com/"));

            // Case 1

            var result = await wowzaHttpClient.GetStreamRecorderAsync
            (
                It.IsAny<string>()
            );

            result.Should().NotBeNull();
        }

        [Test]
        public void StopStreamRecorderAsync_Throws_AudioPlatformException_On_Http_Failure()
        {
            SetupWithBaseAddress(new Uri($"http://{nameof(Exception)}.com/"));

            // Case 1

            var exception = Assert.ThrowsAsync<Exception>
            (
                () => wowzaHttpClient.StopStreamRecorderAsync
                (
                    It.IsAny<string>()
                )
            );
            exception.Message.Should().Be("Exception thrown");
        }

        [Test]
        public void StopStreamRecorderAsync_Success()
        {
            SetupWithBaseAddress(new Uri($"http://{HttpStatusCode.OK}.com/"));

            // Case 1

            var result = wowzaHttpClient.StopStreamRecorderAsync
            (
                It.IsAny<string>()
            );

            result.IsCompleted.Should().BeTrue();
            result.IsFaulted.Should().BeFalse();
        }

        [Test]
        public async Task GetApplicationsDisgnosticsAsync_Success()
        {
            SetupWithBaseAddress(new Uri($"http://{HttpStatusCode.OK}.com/"));

            // Case 1
            var result = await wowzaHttpClient.GetDiagnosticsAsync();
            result.Should().NotBeNull();
        }

        [Test]
        public void GetApplicationsDisgnosticsAsync_Throws_AudioPlatformException_On_Http_Failure()
        {
            Exception exception;
            SetupWithBaseAddress(new Uri($"http://{nameof(Exception)}.com/"));

            // Case 1
            exception = Assert.ThrowsAsync<Exception>
            (
                () => wowzaHttpClient.GetDiagnosticsAsync()
            );
            exception.Message.Should().Be("Exception thrown");
        }
    }
}
