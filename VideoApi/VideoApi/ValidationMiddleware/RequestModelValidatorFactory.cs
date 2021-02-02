using System;
using FluentValidation;

namespace VideoApi.ValidationMiddleware
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
