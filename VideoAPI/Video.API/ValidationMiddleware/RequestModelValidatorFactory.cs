using System;
using FluentValidation;

namespace Video.API.ValidationMiddleware
{
    public class RequestModelValidatorFactory : ValidatorFactoryBase
    {
        private readonly IServiceProvider _serviceProvider;

        public RequestModelValidatorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override IValidator CreateInstance(Type validatorType)
        {
            var validator = (IValidator)_serviceProvider.GetService(validatorType);
            return validator;
        }
    }
}