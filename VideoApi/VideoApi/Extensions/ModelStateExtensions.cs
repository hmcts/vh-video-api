using System.Collections.Generic;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace VideoApi.Extensions
{
    public static class ModelStateExtensions
    {
        public static void AddFluentValidationErrors(this ModelStateDictionary modelState, IEnumerable<ValidationFailure> validationFailures)
        {
            foreach (var failure in validationFailures)
            {
                modelState.AddModelError(failure.PropertyName, failure.ErrorMessage);
            }
        }
    }
}
