﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using VideoApi.Common.Security.Supplier.Kinly;
using VideoApi.Services.Handlers.Kinly;
using VideoApi.UnitTests.Clients;

namespace VideoApi.UnitTests.Services.Handlers
{
    public class KinlySelfTestApiDelegatingHandlerTest
    {
        private readonly Mock<IKinlyJwtProvider> _customJwtTokenProvider;
        private readonly string _stringToken;

        public KinlySelfTestApiDelegatingHandlerTest()
        {
            _customJwtTokenProvider = new Mock<IKinlyJwtProvider>();

            _stringToken = "StringToken";
            _customJwtTokenProvider.Setup(x => x.GenerateSelfTestApiToken(It.IsAny<string>(), It.IsAny<int>())).Returns(_stringToken);
        }
        
        [Test]
        public async Task Should_send_http_request()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://somesite.com");
            request.Options.Set(new HttpRequestOptionsKey<Guid>("participantId"), Guid.NewGuid());
            
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

            Assert.ThrowsAsync<KeyNotFoundException>(() => invoker.SendAsync(request, new CancellationToken()));
        }
    }
}
