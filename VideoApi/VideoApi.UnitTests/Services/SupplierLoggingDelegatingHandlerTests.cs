using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using VideoApi.Services.Handlers;

namespace VideoApi.UnitTests.Services;

public class SupplierLoggingDelegatingHandlerTests
{
    private Mock<ILogger<SupplierLoggingDelegatingHandler>> _loggerMock;
    private TelemetryClient _telemetryClient;
    private Mock<HttpMessageHandler> _mockInnerHandler;
    private SupplierLoggingDelegatingHandler _handler;
    
    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<SupplierLoggingDelegatingHandler>>();
        _telemetryClient = new TelemetryClient(new TelemetryConfiguration());
        _mockInnerHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        _handler = new SupplierLoggingDelegatingHandler(_loggerMock.Object, _telemetryClient)
        {
            InnerHandler = _mockInnerHandler.Object
        };
    }

    [Test]
    public async Task SendAsync_LogsRequestAndResponseDetails()
    {
        // Arrange
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "http://test.com")
        {
            Content = new StringContent("Request")
        };
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("Response")
        };

        _mockInnerHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", requestMessage, ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage)
            .Verifiable();

        // Act
        var invoker = new HttpMessageInvoker(_handler);
        await invoker.SendAsync(requestMessage, new CancellationToken());

        // Assert
        _loggerMock.Verify(x => x.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Request to")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);


        _mockInnerHandler.Protected().Verify("SendAsync", Times.Once(), requestMessage, ItExpr.IsAny<CancellationToken>());
    }
}
