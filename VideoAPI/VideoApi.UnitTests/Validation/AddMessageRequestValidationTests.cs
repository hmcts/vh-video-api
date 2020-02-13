using System.Linq;
using System.Threading.Tasks;
using Faker;
using FluentAssertions;
using NUnit.Framework;
using Video.API.Validations;
using VideoApi.Contract.Requests;

namespace VideoApi.UnitTests.Validation
{
    public class AddMessageRequestValidationTests
    {
        private AddMessageRequestValidation _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new AddMessageRequestValidation();
        }

        [Test]
        public async Task should_pass_validation()
        {
            var request = new AddMessageRequest
            {
                From = Internet.Email(),
                To = Internet.Email(),
                MessageText = Internet.DomainWord(),
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task should_fail_validation_with_errors()
        {
            var request = new AddMessageRequest();
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(3);
            result.Errors.Any(x => x.ErrorMessage == AddMessageRequestValidation.NoFromErrorMessage)
                .Should().BeTrue();
            result.Errors.Any(x => x.ErrorMessage == AddMessageRequestValidation.NoToErrorMessage)
                .Should().BeTrue();
            result.Errors.Any(x => x.ErrorMessage == AddMessageRequestValidation.NoMessageTextErrorMessage)
                .Should().BeTrue();
        }
    }
}
