using System;
using FluentAssertions;
using FluentValidation;
using Moq;
using NUnit.Framework;
using Video.API.ValidationMiddleware;
using VideoApi.Contract.Requests;

namespace VideoApi.UnitTests.Middleware
{
    public class RequestModelValidatorServiceTests
    {
        private RequestModelValidatorService _service;
        private Mock<IValidatorFactory> _validatorFactory;

        [SetUp]
        public void Setup()
        {
            _validatorFactory = new Mock<IValidatorFactory>();
            _service = new RequestModelValidatorService(_validatorFactory.Object);
        }
        
        [Test]
        public void should_return_failure_when_request_validator_is_not_found()
        {
            _validatorFactory.Setup(x => x.GetValidator(It.IsAny<Type>())).Returns((IValidator) null);
            var model = new StartHearingRequest();
            
            var result = _service.Validate(model.GetType(), model);

            result.Should().NotBeNullOrEmpty();
        }
    }
}
