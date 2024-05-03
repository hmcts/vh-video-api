using System.Linq;
using System.Threading.Tasks;
using VideoApi.Contract.Requests;
using VideoApi.Validations;

namespace VideoApi.UnitTests.Validation
{
    public class UpdateParticipantUsernameRequestValidationTests
    {
        private UpdateParticipantUsernameRequestValidation _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new UpdateParticipantUsernameRequestValidation();
        }

        [Test]
        public async Task Should_pass_validation_when_request_is_valid()
        {
            var request = new UpdateParticipantUsernameRequest
            {
                Username = "NewUsername"
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [TestCase(null)]
        [TestCase("")]
        public async Task Should_fail_validation_when_username_is_null_or_empty(string username)
        {
            var request = new UpdateParticipantUsernameRequest
            {
                Username = username
            };

            var result = await _validator.ValidateAsync(request);

            result.Errors.Any(x => x.ErrorMessage == UpdateParticipantUsernameRequestValidation.NoUsernameErrorMessage)
                .Should().BeTrue();
        }
    }
}
