using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using VideoApi.Services.Handlers;

namespace VideoApi.UnitTests.Services;

public class SupplierLoggingDelegatingHandlerTests
{
    private ActivityListener _activityListener;
    private SupplierLoggingDelegatingHandler _handler;
    private Mock<ILogger<SupplierLoggingDelegatingHandler>> _loggerMock;
    private Mock<HttpMessageHandler> _mockInnerHandler;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<SupplierLoggingDelegatingHandler>>();
        _loggerMock.Setup(x => x.IsEnabled(LogLevel.Error)).Returns(true);
        _loggerMock.Setup(x => x.IsEnabled(LogLevel.Warning)).Returns(true);
        _loggerMock.Setup(x => x.IsEnabled(LogLevel.Information)).Returns(true);
        _mockInnerHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        _handler = new SupplierLoggingDelegatingHandler(_loggerMock.Object)
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

    [Test]
    public void verify_activity_tracing()
    {
        var capturedActivity = new Activity("SendToSupplier");
        _activityListener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "SupplierLoggingDelegatingHandler",
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
            ActivityStarted = activity => capturedActivity = activity,
            ActivityStopped = activity => { }
        };
        ActivitySource.AddActivityListener(_activityListener);
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
        invoker.SendAsync(requestMessage, new CancellationToken()).Wait();

        // Assert
        capturedActivity.Should().NotBeNull("an Activity should be created during request execution");
        capturedActivity!.OperationName.Should().Be("SendToSupplier");
        capturedActivity.Kind.Should().Be(ActivityKind.Client);

        capturedActivity.Tags.Should().Contain(t => t.Key == "http.method" && t.Value == "POST",
            "Activity should capture the HTTP method as a tag");

        capturedActivity.Tags.Should().Contain(t => t.Key == "http.url" && t.Value == "http://test.com/",
            "Activity should capture the request URL");

        _mockInnerHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
            ItExpr.IsAny<CancellationToken>()
        );
    }
}
