using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using VideoApi.Common.Security.Kinly;
using VideoApi.Services.Handlers;
using VideoApi.UnitTests.Clients;

namespace VideoApi.UnitTests.Services.Handlers
{
    public class KinlySelfTestApiDelegatingHandlerTest
    {
        private readonly Mock<ICustomJwtTokenProvider> _customJwtTokenProvider;
        private readonly string _stringToken;

        public KinlySelfTestApiDelegatingHandlerTest()
        {
            _customJwtTokenProvider = new Mock<ICustomJwtTokenProvider>();

            _stringToken = "StringToken";
            _customJwtTokenProvider.Setup(x => x.GenerateSelfTestApiToken(It.IsAny<string>(), It.IsAny<int>())).Returns(_stringToken);
        }
        
        [Test]
        public async Task Should_send_http_request()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://somesite.com");
            request.Properties.Add("participantId", Guid.NewGuid());
            
            var handler = new KinlySelfTestApiDelegatingHandler(_customJwtTokenProvider.Object)
            {
                InnerHandler = new FakeHttpMessageHandler()
            };

            var invoker = new HttpMessageInvoker(handler);
            var response = await invoker.SendAsync(request, new CancellationToken());

            request.Headers.Authorization.Parameter.Should().Be(_stringToken);
            response.IsSuccessStatusCode.Should().BeTrue();
        }

        [Test]
        public void Should_throw_exception_when_property_not_in_request_dictionary()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://somesite.com");

            var handler = new KinlySelfTestApiDelegatingHandler(_customJwtTokenProvider.Object)
            {
                InnerHandler = new FakeHttpMessageHandler()
            };

            var invoker = new HttpMessageInvoker(handler);

            Assert.ThrowsAsync<Exception>(() => invoker.SendAsync(request, new CancellationToken()));
        }
    }
}
