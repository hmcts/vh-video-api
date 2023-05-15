using System;
using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Results;

namespace VideoApi.Middleware.Validation
{
    public class RequestModelValidatorService : IRequestModelValidatorService
    {
        private readonly IServiceProvider _serviceProvider;

        public RequestModelValidatorService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IList<ValidationFailure> Validate(Type requestModel, object modelValue)
        {
            var genericType = typeof(IValidator<>).MakeGenericType(requestModel);
            var validator = _serviceProvider.GetService(genericType) as IValidator;
            if (validator == null)
            {
                var failure = new ValidationFailure(modelValue.GetType().ToString(), "Validator not found for request");
                return new List<ValidationFailure>{failure};
            }
            var context = new ValidationContext<object>(modelValue);
            var result = validator.Validate(context);
            return result.Errors;
        }
    }
}
