using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using VideoApi.Services.Clients;
using VideoApi.Services.Exceptions;

namespace VideoApi.UnitTests.Clients
{
    [TestFixture]
    public class WowzaHttpClientTest
    { 
        [Test]
        public void CreateApplicationAsync_Throws_AudioPlatformException_On_Http_Failure()
        {
            WowzaHttpClient wowzaHttpClient;
            AudioPlatformException exception;
            
            // Case 1
            wowzaHttpClient = new WowzaHttpClient(new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri($"http://{nameof(Exception)}.com/")
            });
            
            exception = Assert.ThrowsAsync<AudioPlatformException>
            (
                () => wowzaHttpClient.CreateApplicationAsync
                (
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
                )
            );
            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Message.Should().Be("Exception thrown");
            
            // Case 2
            wowzaHttpClient = new WowzaHttpClient(new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri($"http://{HttpStatusCode.BadRequest}.com/")
            });
            
            exception = Assert.ThrowsAsync<AudioPlatformException>
            (
                () => wowzaHttpClient.CreateApplicationAsync
                (
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
                )
            );
            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Message.Should().Be("Bad request");
        }
        
        [Test]
        public void CreateApplicationAsync_Success()
        {
            // Case 1
            var wowzaHttpClient = new WowzaHttpClient(new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri($"http://{HttpStatusCode.OK}.com/")
            });

            var result = wowzaHttpClient.CreateApplicationAsync
            (
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
            );

            result.IsCompleted.Should().BeTrue();
            result.IsFaulted.Should().BeFalse();
        }

        [Test]
        public void UpdateApplicationAsync_Throws_AudioPlatformException_On_Http_Failure()
        {
            WowzaHttpClient wowzaHttpClient;
            AudioPlatformException exception;

            // Case 1
            wowzaHttpClient = new WowzaHttpClient(new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri($"http://{nameof(Exception)}.com/")
            });

            exception = Assert.ThrowsAsync<AudioPlatformException>
            (
                () => wowzaHttpClient.UpdateApplicationAsync
                (
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
                )
            );
            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Message.Should().Be("Exception thrown");

            // Case 2
            wowzaHttpClient = new WowzaHttpClient(new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri($"http://{HttpStatusCode.BadRequest}.com/")
            });

            exception = Assert.ThrowsAsync<AudioPlatformException>
            (
                () => wowzaHttpClient.UpdateApplicationAsync
                (
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
                )
            );
            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Message.Should().Be("Bad request");
        }

        [Test]
        public void UpdateApplicationAsync_Success()
        {
            // Case 1
            var wowzaHttpClient = new WowzaHttpClient(new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri($"http://{HttpStatusCode.OK}.com/")
            });

            var result = wowzaHttpClient.UpdateApplicationAsync
            (
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
            );

            result.IsCompleted.Should().BeTrue();
            result.IsFaulted.Should().BeFalse();
        }

        [Test]
        public void DeleteApplicationAsync_Throws_AudioPlatformException_On_Http_Failure()
        {
            WowzaHttpClient wowzaHttpClient;
            AudioPlatformException exception;
            
            // Case 1
            wowzaHttpClient = new WowzaHttpClient(new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri($"http://{nameof(Exception)}.com/")
            });
            
            exception = Assert.ThrowsAsync<AudioPlatformException>
            (
                () => wowzaHttpClient.DeleteApplicationAsync
                (
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
                )
            );
            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Message.Should().Be("Exception thrown");
            
            // Case 2
            wowzaHttpClient = new WowzaHttpClient(new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri($"http://{HttpStatusCode.BadRequest}.com/")
            });
            
            exception = Assert.ThrowsAsync<AudioPlatformException>
            (
                () => wowzaHttpClient.DeleteApplicationAsync
                (
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
                )
            );
            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Message.Should().Be("Bad request");
        }
        
        [Test]
        public void DeleteApplicationAsync_Success()
        {
            // Case 1
            var wowzaHttpClient = new WowzaHttpClient(new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri($"http://{HttpStatusCode.OK}.com/")
            });

            var result = wowzaHttpClient.DeleteApplicationAsync
            (
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
            );

            result.IsCompleted.Should().BeTrue();
            result.IsFaulted.Should().BeFalse();
        }
        
        [Test]
        public void AddStreamRecorderAsync_Throws_AudioPlatformException_On_Http_Failure()
        {
            WowzaHttpClient wowzaHttpClient;
            AudioPlatformException exception;
            
            // Case 1
            wowzaHttpClient = new WowzaHttpClient(new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri($"http://{nameof(Exception)}.com/")
            });
            
            exception = Assert.ThrowsAsync<AudioPlatformException>
            (
                () => wowzaHttpClient.AddStreamRecorderAsync
                (
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
                )
            );
            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Message.Should().Be("Exception thrown");
            
            // Case 2
            wowzaHttpClient = new WowzaHttpClient(new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri($"http://{HttpStatusCode.BadRequest}.com/")
            });
            
            exception = Assert.ThrowsAsync<AudioPlatformException>
            (
                () => wowzaHttpClient.AddStreamRecorderAsync
                (
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
                )
            );
            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Message.Should().Be("Bad request");
        }
        
        [Test]
        public void AddStreamRecorderAsync_Success()
        {
            // Case 1
            var wowzaHttpClient = new WowzaHttpClient(new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri($"http://{HttpStatusCode.OK}.com/")
            });

            var result = wowzaHttpClient.AddStreamRecorderAsync
            (
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
            );

            result.IsCompleted.Should().BeTrue();
            result.IsFaulted.Should().BeFalse();
        }
        
        [Test]
        public void MonitoringStreamRecorderAsync_Throws_AudioPlatformException_On_Http_Failure()
        {
            WowzaHttpClient wowzaHttpClient;
            AudioPlatformException exception;
            
            // Case 1
            wowzaHttpClient = new WowzaHttpClient(new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri($"http://{nameof(Exception)}.com/")
            });
            
            exception = Assert.ThrowsAsync<AudioPlatformException>
            (
                () => wowzaHttpClient.MonitoringStreamRecorderAsync
                (
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
                )
            );
            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Message.Should().Be("Exception thrown");
            
            // Case 2
            wowzaHttpClient = new WowzaHttpClient(new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri($"http://{HttpStatusCode.BadRequest}.com/")
            });
            
            exception = Assert.ThrowsAsync<AudioPlatformException>
            (
                () => wowzaHttpClient.MonitoringStreamRecorderAsync
                (
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
                )
            );
            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Message.Should().Be("Bad request");
        }
        
        [Test]
        public async Task  MonitoringStreamRecorderAsync_Success()
        {
            // Case 1
            var wowzaHttpClient = new WowzaHttpClient(new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri($"http://{HttpStatusCode.OK}.com/")
            });

            var result = await wowzaHttpClient.MonitoringStreamRecorderAsync
            (
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
            );

            result.Should().NotBeNull();
        }
        
        [Test]
        public void GetApplicationsAsync_Throws_AudioPlatformException_On_Http_Failure()
        {
            WowzaHttpClient wowzaHttpClient;
            AudioPlatformException exception;
            
            // Case 1
            wowzaHttpClient = new WowzaHttpClient(new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri($"http://{nameof(Exception)}.com/")
            });
            
            exception = Assert.ThrowsAsync<AudioPlatformException>
            (
                () => wowzaHttpClient.GetApplicationsAsync
                (
                    It.IsAny<string>(), It.IsAny<string>()
                )
            );
            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Message.Should().Be("Exception thrown");
            
            // Case 2
            wowzaHttpClient = new WowzaHttpClient(new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri($"http://{HttpStatusCode.BadRequest}.com/")
            });
            
            exception = Assert.ThrowsAsync<AudioPlatformException>
            (
                () => wowzaHttpClient.GetApplicationsAsync
                (
                    It.IsAny<string>(), It.IsAny<string>()
                )
            );
            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Message.Should().Be("Bad request");
        }
        
        [Test]
        public async Task  GetApplicationsAsync_Success()
        {
            // Case 1
            var wowzaHttpClient = new WowzaHttpClient(new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri($"http://{HttpStatusCode.OK}.com/")
            });

            var result = await wowzaHttpClient.GetApplicationsAsync
            (
                It.IsAny<string>(), It.IsAny<string>()
            );

            result.Should().NotBeNull();
        }
        
        [Test]
        public void GetApplicationAsync_Throws_AudioPlatformException_On_Http_Failure()
        {
            WowzaHttpClient wowzaHttpClient;
            AudioPlatformException exception;
            
            // Case 1
            wowzaHttpClient = new WowzaHttpClient(new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri($"http://{nameof(Exception)}.com/")
            });
            
            exception = Assert.ThrowsAsync<AudioPlatformException>
            (
                () => wowzaHttpClient.GetApplicationAsync
                (
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
                )
            );
            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Message.Should().Be("Exception thrown");
            
            // Case 2
            wowzaHttpClient = new WowzaHttpClient(new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri($"http://{HttpStatusCode.BadRequest}.com/")
            });
            
            exception = Assert.ThrowsAsync<AudioPlatformException>
            (
                () => wowzaHttpClient.GetApplicationAsync
                (
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
                )
            );
            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Message.Should().Be("Bad request");
        }
        
        [Test]
        public async Task GetApplicationAsync_Success()
        {
            // Case 1
            var wowzaHttpClient = new WowzaHttpClient(new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri($"http://{HttpStatusCode.OK}.com/")
            });

            var result = await wowzaHttpClient.GetApplicationAsync
            (
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
            );

            result.Should().NotBeNull();
        }
        
        [Test]
        public void GetStreamRecorderAsync_Throws_AudioPlatformException_On_Http_Failure()
        {
            WowzaHttpClient wowzaHttpClient;
            AudioPlatformException exception;
            
            // Case 1
            wowzaHttpClient = new WowzaHttpClient(new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri($"http://{nameof(Exception)}.com/")
            });
            
            exception = Assert.ThrowsAsync<AudioPlatformException>
            (
                () => wowzaHttpClient.GetStreamRecorderAsync
                (
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
                )
            );
            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Message.Should().Be("Exception thrown");
            
            // Case 2
            wowzaHttpClient = new WowzaHttpClient(new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri($"http://{HttpStatusCode.BadRequest}.com/")
            });
            
            exception = Assert.ThrowsAsync<AudioPlatformException>
            (
                () => wowzaHttpClient.GetStreamRecorderAsync
                (
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
                )
            );
            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Message.Should().Be("Bad request");
        }
        
        [Test]
        public async Task GetStreamRecorderAsync_Success()
        {
            // Case 1
            var wowzaHttpClient = new WowzaHttpClient(new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri($"http://{HttpStatusCode.OK}.com/")
            });

            var result = await wowzaHttpClient.GetStreamRecorderAsync
            (
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
            );

            result.Should().NotBeNull();
        }
        
        [Test]
        public void StopStreamRecorderAsync_Throws_AudioPlatformException_On_Http_Failure()
        {
            WowzaHttpClient wowzaHttpClient;
            AudioPlatformException exception;
            
            // Case 1
            wowzaHttpClient = new WowzaHttpClient(new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri($"http://{nameof(Exception)}.com/")
            });
            
            exception = Assert.ThrowsAsync<AudioPlatformException>
            (
                () => wowzaHttpClient.StopStreamRecorderAsync
                (
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
                )
            );
            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Message.Should().Be("Exception thrown");
            
            // Case 2
            wowzaHttpClient = new WowzaHttpClient(new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri($"http://{HttpStatusCode.BadRequest}.com/")
            });
            
            exception = Assert.ThrowsAsync<AudioPlatformException>
            (
                () => wowzaHttpClient.StopStreamRecorderAsync
                (
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
                )
            );
            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Message.Should().Be("Bad request");
        }
        
        [Test]
        public void StopStreamRecorderAsync_Success()
        {
            // Case 1
            var wowzaHttpClient = new WowzaHttpClient(new HttpClient(new FakeHttpMessageHandler())
            {
                BaseAddress = new Uri($"http://{HttpStatusCode.OK}.com/")
            });

            var result = wowzaHttpClient.StopStreamRecorderAsync
            (
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
            );

            result.IsCompleted.Should().BeTrue();
            result.IsFaulted.Should().BeFalse();
        }
    }
}
