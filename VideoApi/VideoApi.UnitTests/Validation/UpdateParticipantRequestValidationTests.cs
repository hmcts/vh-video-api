using System.Linq;
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
        public async Task Should_return_error_no_display_name()
        {
            var request = new UpdateParticipantRequest();
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Any(x => x.ErrorMessage == UpdateParticipantRequestValidation.NoDisplayNameErrorMessage).Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_error_invalid_regex()
        {
            var request = new UpdateParticipantRequest
            {
                DisplayName = "displayname$"
            };
            var result = await _validator.ValidateAsync(request);
            
            result.IsValid.Should().BeFalse();
            result.Errors.Any(x => x.ErrorMessage == UpdateParticipantRequestValidation.InvalidDisplayNameErrorMessage).Should().BeTrue();
        }
        
        private UpdateParticipantRequest BuildRequest()
        {
            return new UpdateParticipantRequest {
                DisplayName = "displayname", 
                Fullname = "Fullname", 
                FirstName = "FirstName", 
                LastName = "LastName",
                ContactEmail = "prson@hmcts.net",
                ContactTelephone = "098765432",
                Username = "username@hmcts.net"
            };
        }
    }
}
