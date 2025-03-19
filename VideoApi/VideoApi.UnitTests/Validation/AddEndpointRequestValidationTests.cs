using System.Threading.Tasks;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using VideoApi.Validations;

namespace VideoApi.UnitTests.Validation
{
    public class AddEndpointRequestValidationTests
    {
        private readonly string _validSipAddress = "2834712384@test.net";
        private AddEndpointRequestValidation _validator;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new AddEndpointRequestValidation();
        }
        
        [Test]
        public async Task Should_pass_validation()
        {
            var request = new AddEndpointRequest
            {
                DisplayName = "Display name",
                Pin = "1234",
                SipAddress = _validSipAddress, 
                ConferenceRole = ConferenceRole.Guest
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
        
        [Test]
        public async Task Should_fail_validation_when_display_name_is_empty()
        {
            var request = new AddEndpointRequest
            {
                DisplayName = string.Empty,
                Pin = "1234",
                SipAddress = _validSipAddress, 
                ConferenceRole = ConferenceRole.Guest
            };

            var result = await _validator.ValidateAsync(request);
            result.Errors.Exists(x => x.ErrorMessage == AddEndpointRequestValidation.NoDisplayNameError)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_fail_validation_when_pin_is_empty()
        {
            var request = new AddEndpointRequest
            {
                DisplayName = "Display name",
                Pin = string.Empty,
                SipAddress = _validSipAddress, 
                ConferenceRole = ConferenceRole.Guest
            };

            var result = await _validator.ValidateAsync(request);
            result.Errors.Exists(x => x.ErrorMessage == AddEndpointRequestValidation.NoPinError)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_fail_validation_when_sip_address_is_empty()
        {
            var request = new AddEndpointRequest
            {
                DisplayName = "Display name",
                Pin = "1234",
                SipAddress = string.Empty, 
                ConferenceRole = ConferenceRole.Guest
            };

            var result = await _validator.ValidateAsync(request);
            result.Errors.Exists(x => x.ErrorMessage == AddEndpointRequestValidation.NoSipError)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task should_fail_validation_when_sip_address_is_not_a_correct_format()
        {
            var request = new AddEndpointRequest
            {
                DisplayName = "Display name",
                Pin = "1234",
                SipAddress = "9f90f88-fc7f-4874-9837-669400385e49@test.net", 
                ConferenceRole = ConferenceRole.Guest
            };

            var result = await _validator.ValidateAsync(request);
            result.Errors.Exists(x => x.ErrorMessage == AddEndpointRequestValidation.SipFormatError)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task should_fail_when_conference_role_is_invalid()
        {
            var request = new AddEndpointRequest
            {
                DisplayName = "Display name",
                Pin = "1234",
                SipAddress = _validSipAddress
            };
            
            var result = await _validator.ValidateAsync(request);
            result.Errors.Exists(x => x.ErrorMessage == AddEndpointRequestValidation.InvalidRoleError)
                .Should().BeTrue();
        }
    }
}
