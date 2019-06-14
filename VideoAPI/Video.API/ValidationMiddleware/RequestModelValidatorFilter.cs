using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Video.API.Extensions;

namespace Video.API.ValidationMiddleware
{
    public class RequestModelValidatorFilter : IAsyncActionFilter
    {
        private readonly IRequestModelValidatorService _requestModelValidatorService;
        private readonly ILogger<RequestModelValidatorFilter> _logger;

        public RequestModelValidatorFilter(IRequestModelValidatorService requestModelValidatorService,
            ILogger<RequestModelValidatorFilter> logger)
        {
            _requestModelValidatorService = requestModelValidatorService;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogDebug($"Processing request {context.ActionDescriptor.DisplayName}");
            foreach (var property in context.ActionDescriptor.Parameters)
            {
                var valuePair = context.ActionArguments.SingleOrDefault(x => x.Key == property.Name);
                if (property.BindingInfo?.BindingSource == BindingSource.Body)
                {
                    var validationFailures = _requestModelValidatorService.Validate(property.ParameterType, valuePair.Value);
                    context.ModelState.AddFluentValidationErrors(validationFailures);
                }
                
                if (valuePair.Value.Equals(GetDefaultValue(property.ParameterType)))
                {
                    context.ModelState.AddModelError(valuePair.Key, $"Please provide a valid {valuePair.Key}");

                }
            }

            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)).ToList();
                _logger.LogError($"Request Validation Failed: {string.Join("; ", errors)}");
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
            else
            {
                await next();
            }
        }

        private static object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}
