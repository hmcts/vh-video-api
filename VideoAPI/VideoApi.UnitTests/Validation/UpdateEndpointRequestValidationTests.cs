using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Video.API.Validations;
using VideoApi.Contract.Requests;

namespace VideoApi.UnitTests.Validation
{
    public class UpdateEndpointRequestValidationTests
    {
        private UpdateEndpointRequestValidation _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new UpdateEndpointRequestValidation();
        }

        [Test]
        public async Task Should_pass_validation_when_displayname_is_populated()
        {
            var request = new UpdateEndpointRequest
            {
                DisplayName = "Updated display name"
            };
            var result = await _validator.ValidateAsync(request);
            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_fail_validation_when_display_name_is_empty()
        {
            var request = new UpdateEndpointRequest
            {
                DisplayName = string.Empty
            };

            var result = await _validator.ValidateAsync(request);
            result.Errors.Any(x => x.ErrorMessage == UpdateEndpointRequestValidation.NoDisplayNameError)
                .Should().BeTrue();
        }
    }
}
