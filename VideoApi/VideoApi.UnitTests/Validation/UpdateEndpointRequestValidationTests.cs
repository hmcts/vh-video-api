using System.Linq;
using System.Threading.Tasks;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using VideoApi.Validations;

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
                DisplayName = "Updated display name",
                ConferenceRole = ConferenceRole.Guest
            };
            var result = await _validator.ValidateAsync(request);
            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_fail_validation_when_display_name_is_empty()
        {
            var request = new UpdateEndpointRequest
            {
                DisplayName = string.Empty,
                ConferenceRole = ConferenceRole.Guest
            };

            var result = await _validator.ValidateAsync(request);
            result.Errors.Any(x => x.ErrorMessage == UpdateEndpointRequestValidation.NoDisplayNameError)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_fail_validation_when_conference_role_is_empty()
        {
            var request = new UpdateEndpointRequest
            {
                DisplayName = "Updated display name",
            };

            var result = await _validator.ValidateAsync(request);
            result.Errors.Any(x => x.ErrorMessage == UpdateEndpointRequestValidation.InvalidRoleError)
                .Should().BeTrue();
        }
    }
}
