using System;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using Video.API.Validations;
using VideoApi.Contract.Requests;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Validation
{
    public class UpdateConferenceRequestValidationTests
    {
        private UpdateConferenceRequestValidation _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new UpdateConferenceRequestValidation();
        }

        private UpdateConferenceRequest BuildRequest()
        {
            var participants = Builder<ParticipantRequest>.CreateListOfSize(4)
                .All().With(x => x.UserRole = UserRole.Individual).Build().ToList();
            return Builder<UpdateConferenceRequest>.CreateNew()
                .With(x => x.ScheduledDateTime = DateTime.Now.AddDays(5))
                .With(x => x.ScheduledDuration = 120)
                .Build();
        }

        [Test]
        public async Task should_pass_validation()
        {
            var request = BuildRequest();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task should_return_missing_hearing_ref_id_error()
        {
            var request = BuildRequest();
            request.HearingRefId = Guid.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == BookNewConferenceRequestValidation.NoHearingRefIdErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task should_return_missing_case_type_error()
        {
            var request = BuildRequest();
            request.CaseType = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == BookNewConferenceRequestValidation.NoCaseTypeErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task should_return_missing_case_number_error()
        {
            var request = BuildRequest();
            request.CaseNumber = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == BookNewConferenceRequestValidation.NoCaseNumberErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task should_return_invalid_datetime_error()
        {
            var request = BuildRequest();
            request.ScheduledDateTime = DateTime.MinValue;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x =>
                    x.ErrorMessage == BookNewConferenceRequestValidation.ScheduleDateTimeInPastErrorMessage)
                .Should().BeTrue();
        }
    }
}