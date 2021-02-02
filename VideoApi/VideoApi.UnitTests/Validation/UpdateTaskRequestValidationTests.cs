using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.Contract.Requests;
using VideoApi.Validations;

namespace VideoApi.UnitTests.Validation
{
    public class UpdateTaskRequestValidationTests
    {
        private UpdateTaskRequestValidation _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new UpdateTaskRequestValidation();
        }

        [Test]
        public async Task Should_pass_validation()
        {
            var request = new UpdateTaskRequest
            {
                UpdatedBy = "foo@foo.com"
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }


        [Test]
        public async Task Should_return_missing_username()
        {
            var request = new UpdateTaskRequest();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors
                .Any(x => x.ErrorMessage == UpdateTaskRequestValidation.NoUsernameErrorMessage)
                .Should().BeTrue();
        }
    }
}
