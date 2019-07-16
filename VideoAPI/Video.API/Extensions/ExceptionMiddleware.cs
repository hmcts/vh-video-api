using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Extensions.Primitives;
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
                if (httpContext.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    httpContext.Request.Headers.TryGetValue("Authorization", out var authHeaderValue);
                    throw new UnauthorizedAccessException(authHeaderValue);
                }
            }
            catch (BadRequestException ex)
            {
                ApplicationLogger.TraceException(TraceCategory.APIException.ToString(), "400 Exception", ex, null, null);
                await HandleExceptionAsync(httpContext, HttpStatusCode.BadRequest, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                ApplicationLogger.TraceException(TraceCategory.APIException.ToString(), "401 Exception", ex, null, null);
                await HandleUnauthorizedExceptionAsync(httpContext, HttpStatusCode.Unauthorized, ex);
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

        private static Task HandleUnauthorizedExceptionAsync(HttpContext context, HttpStatusCode statusCode, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) statusCode;
            context.Request.Headers.TryGetValue("Authorization", out var authHeaderValue);
            return context.Response.WriteAsync($"{authHeaderValue} : {exception.Message}");
        }

    }
}
