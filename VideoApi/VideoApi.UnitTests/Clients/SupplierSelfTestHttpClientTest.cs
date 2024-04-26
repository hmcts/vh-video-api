using System;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using VideoApi.Common.Security.Supplier.Kinly;
using VideoApi.Domain.Enums;
using VideoApi.Services.Clients;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Clients
{
    public class SupplierSelfTestHttpClientTest
    {
        private readonly IOptions<KinlyConfiguration> _kinlyConfigOptions;
        private readonly Mock<ILogger<SupplierSelfTestHttpClient>> _loggerMock;

        public SupplierSelfTestHttpClientTest()
        {
            _kinlyConfigOptions = Options.Create(new KinlyConfiguration());
            _loggerMock = new Mock<ILogger<SupplierSelfTestHttpClient>>();
        }
        
        [Test]
        public async Task GetTestCallScoreAsync_returns_null_on_not_found()
        {
            _kinlyConfigOptions.Value.SelfTestApiUrl = $"http://{HttpStatusCode.NotFound}.com/";
            var client = new SupplierSelfTestHttpClient(new HttpClient(new FakeHttpMessageHandler()), _kinlyConfigOptions, _loggerMock.Object);

            var result = await client.GetTestCallScoreAsync(It.IsAny<Guid>());

            result.Should().BeNull();
        }

        [Test]
        public async Task GetTestCallScoreAsync_test_call_result_object_passed_good()
        {
            _kinlyConfigOptions.Value.SelfTestApiUrl = $"http://{HttpStatusCode.OK}.com/";
            var client = new SupplierSelfTestHttpClient(new HttpClient(new FakeHttpMessageHandler
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
