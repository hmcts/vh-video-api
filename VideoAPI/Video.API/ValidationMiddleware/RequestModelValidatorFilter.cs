using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Video.API.Extensions;

namespace Video.API.ValidationMiddleware
{
    public class RequestModelValidatorFilter : IAsyncActionFilter
    {
        private readonly IRequestModelValidatorService _requestModelValidatorService;

        public RequestModelValidatorFilter(IRequestModelValidatorService requestModelValidatorService)
        {
            _requestModelValidatorService = requestModelValidatorService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var property in context.ActionDescriptor.Parameters)
            {
                var valuePair = context.ActionArguments.SingleOrDefault(x => x.Key == property.Name);
                if (property.BindingInfo?.BindingSource == BindingSource.Body)
                {
                    var validationFailures = _requestModelValidatorService.Validate(property.ParameterType, valuePair.Value);
                    context.ModelState.AddFluentValidationErrors(validationFailures);
                }
                else if (valuePair.Value.Equals(GetDefaultValue(property.ParameterType)))
                {
                    context.ModelState.AddModelError(valuePair.Key, $"Please provide a valid {valuePair.Key}");

                }
            }

            if (!context.ModelState.IsValid)
            {
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
