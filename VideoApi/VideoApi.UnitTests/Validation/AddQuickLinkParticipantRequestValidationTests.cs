using Bogus;
using FizzWare.NBuilder;
using System.Threading.Tasks;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using VideoApi.Validations;

namespace VideoApi.UnitTests.Validation
{
    public class AddQuickLinkParticipantRequestValidationTests
    {
        private AddQuickLinkParticipantRequestValidation _validator;
        private static readonly Faker Faker = new();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new AddQuickLinkParticipantRequestValidation();
        }

        [Test]
        public async Task Should_pass_validation()
        {
            var request = BuildRequest();

            request.Name = "Peter Costello";
            request.UserRole = UserRole.QuickLinkParticipant;

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
            result.Errors.Exists(x => x.ErrorMessage == AddQuickLinkParticipantRequestValidation.NoNameErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_special_chars_not_allowed_name_error()
        {
            foreach (var specialCase in AddQuickLinkParticipantRequestValidation.invalidSpecialCases)
            {
                var request = BuildRequest();
                request.Name = $"Peter Co{specialCase}tello";

                var result = await _validator.ValidateAsync(request);

                result.IsValid.Should().BeFalse();
                result.Errors.Count.Should().Be(1);
                result.Errors.Exists(x => x.ErrorMessage == AddQuickLinkParticipantRequestValidation.SpecialCharNameErrorMessage)
                    .Should().BeTrue();
            }
        }

        [Test]
        public async Task Should_return_missing_user_role_error()
        {
            var request = BuildRequest();
            request.Name = "Peter Costello";
            request.UserRole = UserRole.None;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Exists(x => x.ErrorMessage == AddQuickLinkParticipantRequestValidation.NoUserRoleErrorMessage)
                .Should().BeTrue();
        }
        
        private static AddQuickLinkParticipantRequest BuildRequest()
        {
            return Builder<AddQuickLinkParticipantRequest>.CreateNew()
                .With(x => x.UserRole = UserRole.Representative)
                .With(x => x.Name = Faker.Name.FullName())
                .Build();
        }
    }
}
