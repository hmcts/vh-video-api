using Faker;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using VideoApi.Validations;

namespace VideoApi.UnitTests.Validation
{
    public class AddMagicLinkParticipantRequestValidationTests
    {
        private AddMagicLinkParticipantRequestValidation _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new AddMagicLinkParticipantRequestValidation();
        }

        [Test]
        public async Task Should_pass_validation()
        {
            var request = BuildRequest();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_return_missing_name_error()
        {
            var request = BuildRequest();
            request.Name = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == AddMagicLinkParticipantRequestValidation.NoNameErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_missing_user_role_error()
        {
            var request = BuildRequest();
            request.UserRole = UserRole.None;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == AddMagicLinkParticipantRequestValidation.NoUserRoleErrorMessage)
                .Should().BeTrue();
        }
        
        private AddMagicLinkParticipantRequest BuildRequest()
        {
            return Builder<AddMagicLinkParticipantRequest>.CreateNew()
                .With(x => x.UserRole = UserRole.Representative)
                .With(x => x.Name = Name.FullName())
                .Build();
        }
    }
}
