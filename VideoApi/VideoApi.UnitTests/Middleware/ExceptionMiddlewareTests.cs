using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using VideoApi.Common;
using VideoApi.Extensions;

namespace VideoApi.UnitTests.Middleware
{
    [TestFixture]
    public class ExceptionMiddlewareTests
    {
        [SetUp]
        public void ExceptionMiddleWareSetup()
        {
            RequestDelegateMock = new Mock<IDelegateMock>();
            HttpContext = new DefaultHttpContext();
        }

        public Mock<IDelegateMock> RequestDelegateMock { get; set; }
        public ExceptionMiddleware ExceptionMiddleware { get; set; }
        public HttpContext HttpContext { get; set; }

        public interface IDelegateMock
        {
            Task RequestDelegate(HttpContext context);
        }

        [Test]
        public async Task Should_Invoke_Delegate()
        {
            RequestDelegateMock
                .Setup(x => x.RequestDelegate(It.IsAny<HttpContext>()))
                .Returns(Task.FromResult(0));
            ExceptionMiddleware = new ExceptionMiddleware(RequestDelegateMock.Object.RequestDelegate);
            await ExceptionMiddleware.InvokeAsync(new DefaultHttpContext());
            RequestDelegateMock.Verify(x => x.RequestDelegate(It.IsAny<HttpContext>()), Times.Once);
        }

        [Test]
        public async Task Should_return_bad_request_message()
        {
            RequestDelegateMock
                .Setup(x => x.RequestDelegate(It.IsAny<HttpContext>()))
                .Returns(Task.FromException(new BadRequestException("Error")));
            ExceptionMiddleware = new ExceptionMiddleware(RequestDelegateMock.Object.RequestDelegate);


            await ExceptionMiddleware.InvokeAsync(HttpContext);

            ClassicAssert.AreEqual((int) HttpStatusCode.BadRequest, HttpContext.Response.StatusCode);
        }

        [Test]
        public async Task Should_return_exception_message()
        {
            RequestDelegateMock
                .Setup(x => x.RequestDelegate(It.IsAny<HttpContext>()))
                .Returns(Task.FromException(new Exception()));
            ExceptionMiddleware = new ExceptionMiddleware(RequestDelegateMock.Object.RequestDelegate);
            
            await ExceptionMiddleware.InvokeAsync(HttpContext);
            ClassicAssert.AreEqual("application/json; charset=utf-8", HttpContext.Response.ContentType);
            ClassicAssert.AreEqual((int) HttpStatusCode.InternalServerError, HttpContext.Response.StatusCode);
        }
    }
}
