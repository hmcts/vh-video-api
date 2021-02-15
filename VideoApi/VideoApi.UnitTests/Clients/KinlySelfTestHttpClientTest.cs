using System;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using VideoApi.Common.Security.Kinly;
using VideoApi.Domain.Enums;
using VideoApi.Services.Clients;
using VideoApi.Services.Kinly;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Clients
{
    public class KinlySelfTestHttpClientTest
    {
        private readonly IOptions<KinlyConfiguration> _kinlyConfigOptions;
        private readonly Mock<ILogger<KinlySelfTestHttpClient>> _loggerMock;

        public KinlySelfTestHttpClientTest()
        {
            _kinlyConfigOptions = Options.Create(new KinlyConfiguration());
            _loggerMock = new Mock<ILogger<KinlySelfTestHttpClient>>();
        }
        
        [Test]
        public async Task GetTestCallScoreAsync_returns_null_on_not_found()
        {
            _kinlyConfigOptions.Value.KinlySelfTestApiUrl = $"http://{HttpStatusCode.NotFound}.com/";
            var client = new KinlySelfTestHttpClient(new HttpClient(new FakeHttpMessageHandler()), _kinlyConfigOptions, _loggerMock.Object);

            var result = await client.GetTestCallScoreAsync(It.IsAny<Guid>());

            result.Should().BeNull();
        }

        [Test]
        public async Task GetTestCallScoreAsync_test_call_result_object_passed_good()
        {
            _kinlyConfigOptions.Value.KinlySelfTestApiUrl = $"http://{HttpStatusCode.OK}.com/";
            var client = new KinlySelfTestHttpClient(new HttpClient(new FakeHttpMessageHandler
            {
                ReturnContent = JsonConvert.SerializeObject(new Testcall{ Passed = true, Score = (int)TestScore.Good, User_id = Guid.NewGuid().ToString() })
            }), _kinlyConfigOptions, _loggerMock.Object);

            var result = await client.GetTestCallScoreAsync(It.IsAny<Guid>());

            result.Should().NotBeNull();
            result.Passed.Should().BeTrue();
            result.Score.Should().NotBeNull();
            result.Score.Should().Be(TestScore.Good);

        }
    }
}
