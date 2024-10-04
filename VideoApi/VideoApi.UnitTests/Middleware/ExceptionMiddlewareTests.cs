using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using VideoApi.Common;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;
using VideoApi.Extensions;
using VideoApi.Services.Clients;

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
        public async Task should_return_bad_request_when_DomainRuleException_is_thrown()
        {
            RequestDelegateMock
                .Setup(x => x.RequestDelegate(It.IsAny<HttpContext>()))
                .Returns(Task.FromException(new DomainRuleException("Error", "Error Test message")));
            ExceptionMiddleware = new ExceptionMiddleware(RequestDelegateMock.Object.RequestDelegate);

            await ExceptionMiddleware.InvokeAsync(HttpContext);
            ClassicAssert.AreEqual("application/json; charset=utf-8", HttpContext.Response.ContentType);
            ClassicAssert.AreEqual((int) HttpStatusCode.BadRequest, HttpContext.Response.StatusCode);
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

        [Test]
        public async Task should_return_bad_request_when_dal_exception_is_thrown()
        {
            RequestDelegateMock
                .Setup(x => x.RequestDelegate(It.IsAny<HttpContext>()))
                .Returns(Task.FromException(new InvalidVirtualCourtRoomTypeException(VirtualCourtRoomType.JudgeJOH, "Wrong one")));
            ExceptionMiddleware = new ExceptionMiddleware(RequestDelegateMock.Object.RequestDelegate);

            await ExceptionMiddleware.InvokeAsync(HttpContext);
            ClassicAssert.AreEqual("application/json; charset=utf-8", HttpContext.Response.ContentType);
            ClassicAssert.AreEqual((int) HttpStatusCode.BadRequest, HttpContext.Response.StatusCode);
        }
        
        [Test]
        public async Task should_return_not_found_when_EntityNotFoundException_is_thrown()
        {
            RequestDelegateMock
                .Setup(x => x.RequestDelegate(It.IsAny<HttpContext>()))
                .Returns(Task.FromException(new ConferenceNotFoundException(Guid.NewGuid())));
            ExceptionMiddleware = new ExceptionMiddleware(RequestDelegateMock.Object.RequestDelegate);

            await ExceptionMiddleware.InvokeAsync(HttpContext);
            ClassicAssert.AreEqual("application/json; charset=utf-8", HttpContext.Response.ContentType);
            ClassicAssert.AreEqual((int) HttpStatusCode.NotFound, HttpContext.Response.StatusCode);
        }

        [Test]
        public async Task should_return_bad_request_when_supplier_api_returns_bad_request()
        {
            var supplierResponseMessage = """
                                          {"timestamp":"2024-07-16T20:53:59.024+0000","errorCode":400,"httpStatus":"BAD_REQUEST","message":"Bad Request","detailedMessage":"Invalid JVS endpoint(s).","path":"/virtual-court/API/v1/hearing"}
                                          """;
            var supplierException = new SupplierApiException("Error", 400, supplierResponseMessage, null, null);
            RequestDelegateMock
                .Setup(x => x.RequestDelegate(It.IsAny<HttpContext>()))
                .Returns(Task.FromException(supplierException));
            ExceptionMiddleware = new ExceptionMiddleware(RequestDelegateMock.Object.RequestDelegate);
            
            await ExceptionMiddleware.InvokeAsync(HttpContext);
            ClassicAssert.AreEqual("application/json; charset=utf-8", HttpContext.Response.ContentType);
            ClassicAssert.AreEqual((int) HttpStatusCode.BadRequest, HttpContext.Response.StatusCode);
        }
        
        [Test]
        public async Task should_return_bad_request_when_supplier_api_returns_other_error()
        {
           
            var supplierException = new SupplierApiException("Error", 500, "random error message", null, null);
            RequestDelegateMock
                .Setup(x => x.RequestDelegate(It.IsAny<HttpContext>()))
                .Returns(Task.FromException(supplierException));
            ExceptionMiddleware = new ExceptionMiddleware(RequestDelegateMock.Object.RequestDelegate);
            
            await ExceptionMiddleware.InvokeAsync(HttpContext);
            ClassicAssert.AreEqual("application/json; charset=utf-8", HttpContext.Response.ContentType);
            ClassicAssert.AreEqual((int) HttpStatusCode.InternalServerError, HttpContext.Response.StatusCode);
        }
    }
}
