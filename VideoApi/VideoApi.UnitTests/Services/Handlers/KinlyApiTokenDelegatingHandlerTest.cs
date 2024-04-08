using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using VideoApi.Common.Security.Supplier.Kinly;
using VideoApi.Services.Handlers;
using VideoApi.UnitTests.Clients;

namespace VideoApi.UnitTests.Services.Handlers
{
    public class KinlyApiTokenDelegatingHandlerTest
    {
        private readonly Mock<IKinlyJwtProvider> _customJwtTokenProvider;
        private readonly string _stringToken;

        public KinlyApiTokenDelegatingHandlerTest()
        {
            _customJwtTokenProvider = new Mock<IKinlyJwtProvider>();

            _stringToken = "StringToken";
            _customJwtTokenProvider.Setup(x => x.GenerateApiToken(It.IsAny<string>(), It.IsAny<int>())).Returns(_stringToken);
        }
        
        [Test]
        public async Task Should_send_http_request()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://somesite.com");
            
            var handler = new KinlyApiTokenDelegatingHandler(_customJwtTokenProvider.Object)
            {
                InnerHandler = new FakeHttpMessageHandler()
            };

            var invoker = new HttpMessageInvoker(handler);
            var response = await invoker.SendAsync(request, new CancellationToken());

            request.Headers.Authorization.Parameter.Should().Be(_stringToken);
            response.IsSuccessStatusCode.Should().BeTrue();
        }
    }
}
