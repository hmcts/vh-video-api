using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Video.API.Validations;
using VideoApi.Contract.Requests;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Validation
{
    public class AddAlertRequestValidationTests
    {
        private AddAlertRequestValidation _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new AddAlertRequestValidation();
        }

        [Test]
        public async Task should_pass_validation()
        {
            var request = BuildRequest();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task should_return_missing_alert_type()
        {
            var request = BuildRequest();
            request.Type = null;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors
                .Any(x => x.ErrorMessage == AddAlertRequestValidation.NoAlertTypeErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task should_return_missing_body()
        {
            var request = BuildRequest();
            request.Body = null;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors
                .Any(x => x.ErrorMessage == AddAlertRequestValidation.NoBodyErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task should_return_missing_username_when_alert_id_is_provided()
        {
            var request = BuildRequest();
            request.AlertId = 1;
            request.UpdatedBy = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors
                .Any(x => x.ErrorMessage == AddAlertRequestValidation.NoUsernameErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task should_return_missing_username_when_alert_id_is_provided_and_invalid_email()
        {
            var request = BuildRequest();
            request.AlertId = 1;
            request.UpdatedBy = "foo";

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors
                .Any(x => x.ErrorMessage == AddAlertRequestValidation.NoUsernameErrorMessage)
                .Should().BeTrue();
        }
        
        private AddAlertRequest BuildRequest()
        {
            return new AddAlertRequest
            {
                Body = "Automated Test",
                Type = AlertType.Judge
            };
        }
    }
}