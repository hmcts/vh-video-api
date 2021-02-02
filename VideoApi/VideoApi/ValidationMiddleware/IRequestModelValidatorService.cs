using System;
using System.Collections.Generic;
using FluentValidation.Results;

namespace Video.API.ValidationMiddleware
{
    public interface IRequestModelValidatorService
    {
        IList<ValidationFailure> Validate(Type requestModel, object modelValue);
    }
}