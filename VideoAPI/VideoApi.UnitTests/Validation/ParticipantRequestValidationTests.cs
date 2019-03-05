using System;
using System.Linq;
using System.Threading.Tasks;
using Faker;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using Video.API.Validations;
using VideoApi.Contract.Requests;

namespace VideoApi.UnitTests.Validation
{
    public class ParticipantRequestValidationTests
    {
        private ParticipantRequestValidation _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new ParticipantRequestValidation();
        }

        [Test]
        public async Task should_pass_validation()
        {
            var request = BuildRequest();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task should_return_missing_participant_ref_id_error()
        {
            var request = BuildRequest();
            request.ParticipantRefId = Guid.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == ParticipantRequestValidation.NoParticipantRefIdErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task should_return_missing_name_error()
        {
            var request = BuildRequest();
            request.Name = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == ParticipantRequestValidation.NoNameErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task should_return_missing_display_name_error()
        {
            var request = BuildRequest();
            request.DisplayName = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == ParticipantRequestValidation.NoDisplayNameErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task should_return_missing_username_error()
        {
            var request = BuildRequest();
            request.Username = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == ParticipantRequestValidation.NoUsernameErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task should_return_missing_case_type_group_error()
        {
            var request = BuildRequest();
            request.CaseTypeGroup = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == ParticipantRequestValidation.NoCaseTypeGroupErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task should_return_missing_hearing_role_error()
        {
            var request = BuildRequest();
            request.HearingRole = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == ParticipantRequestValidation.NoHearingRoleErrorMessage)
                .Should().BeTrue();
        }

        private ParticipantRequest BuildRequest()
        {
            return Builder<ParticipantRequest>.CreateNew()
                .With(x => x.CaseTypeGroup = "Claimant")
                .With(x => x.HearingRole = "Solicitor")
                .With(x => x.Name = Name.FullName())
                .With(x => x.Username = Internet.Email())
                .Build();
        }
    }
}