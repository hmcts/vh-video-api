using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VideoApi.Common.Security.Supplier.Vodafone;
using VideoApi.Domain.Enums;
using VideoApi.Services.Clients;
using VideoApi.Services.Clients.Models;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Clients;

public class SupplierSelfTestHttpClientTest
{
    private readonly Mock<ILogger<SupplierSelfTestHttpClient>> _loggerMock = new();
    private readonly VodafoneConfiguration _vodafoneConfig = Options.Create(new VodafoneConfiguration()).Value;
    
    [Test]
    public async Task GetTestCallScoreAsync_returns_null_on_not_found()
    {
        _vodafoneConfig.SelfTestApiUrl = $"http://{HttpStatusCode.NotFound}.com/";
        var httpClient = new HttpClient(new FakeHttpMessageHandler());
        httpClient.BaseAddress = new Uri(_vodafoneConfig.SelfTestApiUrl);
        var client = new SupplierSelfTestHttpClient(new SupplierApiClient(httpClient), _loggerMock.Object);
        
        var result = await client.GetTestCallScoreAsync(It.IsAny<Guid>());
        
        result.Should().BeNull();
    }
    
    [Test]
    public async Task GetTestCallScoreAsync_test_call_result_object_passed_good()
    {
        _vodafoneConfig.SelfTestApiUrl = $"http://{HttpStatusCode.OK}.com/";
        var httpClient = new HttpClient(new FakeHttpMessageHandler()
        {
            ReturnContent = JsonSerializer.Serialize(new SelfTestParticipantResponse{ Passed = true, Score = (int)TestScore.Good, UserId = Guid.NewGuid() })
        });
        httpClient.BaseAddress = new Uri(_vodafoneConfig.SelfTestApiUrl);
        var client = new SupplierSelfTestHttpClient(new SupplierApiClient(httpClient), _loggerMock.Object);
        
        var result = await client.GetTestCallScoreAsync(It.IsAny<Guid>());
        
        result.Should().NotBeNull();
        result.Passed.Should().BeTrue();
        result.Score.Should().Be(TestScore.Good);
        
    }
}
