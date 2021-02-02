using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.Contract.Requests;
using VideoApi.Validations;

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
        public async Task Should_pass_validation()
        {
            var request = new AddInstantMessageRequest
            {
                From = "Display name",
                MessageText = "This is a test message",
                To = "Receiver display name"
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_fail_validation_when_from_is_empty()
        {
            var request = new AddInstantMessageRequest
            {
                MessageText = "test message",
            };
            var result = await _validator.ValidateAsync(request);
            result.Errors.Any(x => x.ErrorMessage == AddMessageRequestValidation.NoFromErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_fail_validation_when_message_text_is_empty()
        {
            var request = new AddInstantMessageRequest
            {
                From = "Display Name"
            };
            var result = await _validator.ValidateAsync(request);
            result.Errors.Any(x => x.ErrorMessage == AddMessageRequestValidation.NoMessageTextErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_fail_validation_when_message_text_is_too_long()
        {
            var request = new AddInstantMessageRequest
            {
                From = "Display Name",
                MessageText = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum"
            };
            
            var result = await _validator.ValidateAsync(request);
            result.Errors.Any(x => x.ErrorMessage == AddMessageRequestValidation.MaxMessageLength)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_fail_validation_when_to_is_empty()
        {
            var request = new AddInstantMessageRequest
            {
                From = "Display Name",
                MessageText = "test message",
            };
            var result = await _validator.ValidateAsync(request);
            result.Errors.Any(x => x.ErrorMessage == AddMessageRequestValidation.NoToErrorMessage)
                .Should().BeTrue();
        }
    }
}
