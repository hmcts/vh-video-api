using System.Threading.Tasks;
using VideoApi.Contract.Requests;
using VideoApi.Validations;

namespace VideoApi.UnitTests.Validation
{
    public class UpdateParticipantRequestValidationTests
    {
        private UpdateParticipantRequestValidation _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new UpdateParticipantRequestValidation();
        }

        [Test]
        public async Task Should_pass_validation()
        {
            var request = BuildRequest();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

    
        [Test]
        public async Task Should_return_error()
        {
            var request = new UpdateParticipantRequest();
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Exists(x => x.ErrorMessage == UpdateParticipantRequestValidation.NoDisplayNameErrorMessage).Should().BeTrue();
        }

        private static UpdateParticipantRequest BuildRequest()
        {
            return new UpdateParticipantRequest {
                DisplayName = "displayname", 
                ContactEmail = "prson@hmcts.net",
                Username = "username@hmcts.net"
            };
        }
    }
}
