using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.Net;
using VideoApi.Common;
using VideoApi.Common.Helpers;

namespace Video.API.Extensions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (BadRequestException ex)
            {
                ApplicationLogger.TraceException(TraceCategory.APIException.ToString(), "400 Exception", ex, null, null);
                await HandleExceptionAsync(httpContext, HttpStatusCode.BadRequest, ex);
            }
            catch (Exception ex)
            {
                ApplicationLogger.TraceException(TraceCategory.APIException.ToString(), "API Exception", ex,null, null);
                await HandleExceptionAsync(httpContext, HttpStatusCode.InternalServerError, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) statusCode;
                   
            return context.Response.WriteAsync(exception.Message);
        }
    }
}
