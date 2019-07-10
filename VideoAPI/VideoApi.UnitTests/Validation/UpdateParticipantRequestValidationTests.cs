using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Video.API.Validations;
using VideoApi.Contract.Requests;

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
        public async Task should_pass_validation()
        {
            var request = BuildRequest();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

    
        [Test]
        public async Task should_return_error()
        {
            var request = new UpdateParticipantRequest();
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(2);
            result.Errors.Any(x => x.ErrorMessage == UpdateParticipantRequestValidation.NoNameErrorMessage)
                .Should().BeTrue();
            result.Errors.Any(x => x.ErrorMessage == UpdateParticipantRequestValidation.NoDisplayNameErrorMessage)
                .Should().BeTrue();
        }

        private UpdateParticipantRequest BuildRequest()
        {
            return new UpdateParticipantRequest {DisplayName = "displayname", Fullname = "Fullname"};
        }
    }
}