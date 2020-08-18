using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Video.API.Validations;
using VideoApi.Contract.Requests;

namespace VideoApi.UnitTests.Validation
{
    public class AddEndpointRequestValidationTests
    {
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
                SipAddress = "te124@sip.com"
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
                SipAddress = "te124@sip.com"
            };

            var result = await _validator.ValidateAsync(request);
            result.Errors.Any(x => x.ErrorMessage == AddEndpointRequestValidation.NoDisplayNameError)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_fail_validation_when_pin_is_empty()
        {
            var request = new AddEndpointRequest
            {
                DisplayName = "Display name",
                Pin = string.Empty,
                SipAddress = "te124@sip.com"
            };

            var result = await _validator.ValidateAsync(request);
            result.Errors.Any(x => x.ErrorMessage == AddEndpointRequestValidation.NoPinError)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_fail_validation_when_sip_address_is_empty()
        {
            var request = new AddEndpointRequest
            {
                DisplayName = "Display name",
                Pin = "1234",
                SipAddress = string.Empty
            };

            var result = await _validator.ValidateAsync(request);
            result.Errors.Any(x => x.ErrorMessage == AddEndpointRequestValidation.NoSipError)
                .Should().BeTrue();
        }
    }
}
