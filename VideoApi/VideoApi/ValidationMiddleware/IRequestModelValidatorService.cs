using System;
using System.Collections.Generic;
using FluentValidation.Results;

namespace VideoApi.ValidationMiddleware
{
    public interface IRequestModelValidatorService
    {
        IList<ValidationFailure> Validate(Type requestModel, object modelValue);
    }
}
