using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OpenTelemetry.Trace;
using VideoApi.Common;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain.Validations;
using VideoApi.Events.Exceptions;
using VideoApi.Services.Clients;

namespace VideoApi.Extensions;

public class ExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
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
        catch (EntityNotFoundException ex)
        {
            TraceException("Entity Not Found", ex, null, null);
            await HandleExceptionAsync(httpContext, HttpStatusCode.NotFound, ex);
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
            TraceException("Supplier API Exception", ex, null, null);
            if (ex.StatusCode == HttpStatusCode.BadRequest)
            {
                var modelState = new ModelStateDictionary();
                var jsonDoc = JsonDocument.Parse(ex.Response);
                modelState.AddModelError("supplier", jsonDoc.RootElement.ToString());
                var problemDetails = new ValidationProblemDetails(modelState);
                await HandleBadRequestAsync(httpContext, problemDetails);
            }
            else
            {
                await HandleExceptionAsync(httpContext, ex.StatusCode, ex);
            }
        }
        catch (UnexpectedEventOrderException ex)
        {
            TraceException("Unexpected Event Order Exception", ex, null, null);
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int) HttpStatusCode.NoContent;
        }
        catch (Exception ex)
        {
            TraceException("API Exception", ex, null, null);
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
    
    private static void TraceException(string eventTitle, Exception exception, IPrincipal user, IDictionary<string, string> properties)
    {
        var activity = Activity.Current;
        if (activity == null)return;
        
        activity.AddTag("Event", eventTitle);
        activity.RecordException(exception);
        
        if (user?.Identity != null)
            activity.AddTag("User", user.Identity.Name);
        
        if (properties != null)
        {
            foreach (var entry in properties)
            {
                activity.AddTag(entry.Key, entry.Value);
            }
        }
    }
}
