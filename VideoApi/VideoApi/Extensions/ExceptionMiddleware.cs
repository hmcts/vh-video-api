using System;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using VideoApi.Common;
using VideoApi.Common.Helpers;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain.Validations;
using VideoApi.Services.Clients;

namespace VideoApi.Extensions
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
            catch (DomainRuleException ex)
            {
                var modelState = new ModelStateDictionary();
                modelState.AddDomainRuleErrors(ex.ValidationFailures);
                var problemDetails = new ValidationProblemDetails(modelState);
                await HandleBadRequestAsync(httpContext, problemDetails);
            }
            catch (BadRequestException ex)
            {
                var modelState = new ModelStateDictionary();
                modelState.AddModelError("request", ex.Message);
                var problemDetails = new ValidationProblemDetails(modelState);
                await HandleBadRequestAsync(httpContext, problemDetails);
            }
            catch (VideoDalException ex)
            {
                var modelState = new ModelStateDictionary();
                modelState.AddModelError("database", ex.Message);
                var problemDetails = new ValidationProblemDetails(modelState);
                await HandleBadRequestAsync(httpContext, problemDetails);
            }
            catch (SupplierApiException ex)
            {
                ApplicationLogger.TraceException(TraceCategory.SupplierApiException.ToString(),
                    "Supplier API Exception", ex, null, null);
                if (ex.StatusCode == (int)HttpStatusCode.BadRequest)
                {
                    var modelState = new ModelStateDictionary();
                    var jsonDoc = JsonDocument.Parse(ex.Response);
                    modelState.AddModelError("supplier", jsonDoc.RootElement.ToString());
                    var problemDetails = new ValidationProblemDetails(modelState);
                    await HandleBadRequestAsync(httpContext, problemDetails);
                }
                else
                {
                    await HandleExceptionAsync(httpContext, (HttpStatusCode)ex.StatusCode, ex);
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.TraceException(TraceCategory.APIException.ToString(), "API Exception", ex, null,
                    null);
                await HandleExceptionAsync(httpContext, HttpStatusCode.InternalServerError, ex);
            }
        }
        
        private static Task HandleBadRequestAsync(HttpContext httpContext, ValidationProblemDetails problemDetails)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;

            return httpContext.Response.WriteAsJsonAsync(problemDetails);
        }

        private static Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) statusCode;
            var sb = new StringBuilder(exception.Message);
            var innerException = exception.InnerException;
            while (innerException != null)
            {
                sb.Append($" {innerException.Message}");
                innerException = innerException.InnerException;
            }
            return context.Response.WriteAsJsonAsync(sb.ToString());
        }
    }
}
