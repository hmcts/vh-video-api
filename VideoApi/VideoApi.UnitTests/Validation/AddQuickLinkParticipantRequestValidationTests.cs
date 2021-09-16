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
    public class AddQuickLinkParticipantRequestValidationTests
    {
        private AddQuickLinkParticipantRequestValidation _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new AddQuickLinkParticipantRequestValidation();
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
            result.Errors.Any(x => x.ErrorMessage == AddQuickLinkParticipantRequestValidation.NoNameErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_special_chars_not_allowed_name_error()
        {
            var request = BuildRequest();
            request.Name = "#Peter Co$tello";

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == AddQuickLinkParticipantRequestValidation.SpecialCharNameErrorMessage)
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
            result.Errors.Any(x => x.ErrorMessage == AddQuickLinkParticipantRequestValidation.NoUserRoleErrorMessage)
                .Should().BeTrue();
        }
        
        private AddQuickLinkParticipantRequest BuildRequest()
        {
            return Builder<AddQuickLinkParticipantRequest>.CreateNew()
                .With(x => x.UserRole = UserRole.Representative)
                .With(x => x.Name = Name.FullName())
                .Build();
        }
    }
}
