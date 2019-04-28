using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Video.API.Validations;
using VideoApi.Contract.Requests;

namespace VideoApi.UnitTests.Validation
{
    public class UpdateAlertRequestValidationTests
    {
        private UpdateAlertRequestValidation _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new UpdateAlertRequestValidation();
        }

        [Test]
        public async Task should_pass_validation()
        {
            var request = new UpdateAlertRequest
            {
                UpdatedBy = "foo@foo.com"
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }


        [Test]
        public async Task should_return_missing_username()
        {
            var request = new UpdateAlertRequest
            {
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors
                .Any(x => x.ErrorMessage == UpdateAlertRequestValidation.NoUsernameErrorMessage)
                .Should().BeTrue();
        }
    }
}