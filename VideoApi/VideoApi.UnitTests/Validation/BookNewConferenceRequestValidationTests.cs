using System;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Enums;
using VideoApi.Validations;

namespace VideoApi.UnitTests.Validation
{
    public class BookNewConferenceRequestValidationTests
    {
        private BookNewConferenceRequestValidation _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new BookNewConferenceRequestValidation();
        }

        private BookNewConferenceRequest BuildRequest()
        {
            var participants = Builder<ParticipantRequest>.CreateListOfSize(4)
                .All().With(x => x.UserRole = UserRole.Individual).Build().ToList();
            return Builder<BookNewConferenceRequest>.CreateNew()
                .With(x => x.ScheduledDateTime = DateTime.Now.AddDays(5))
                .With(x => x.ScheduledDuration = 120)
                .With(x => x.Participants = participants)
                .Build();
        }

        [Test]
        public async Task Should_pass_validation()
        {
            var request = BuildRequest();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_return_missing_hearing_ref_id_error()
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
        public async Task Should_return_missing_case_type_error()
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
        public async Task Should_return_missing_case_number_error()
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
        public async Task Should_return_invalid_datetime_error()
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

        [Test]
        public async Task Should_return_missing_participants_error()
        {
            var request = BuildRequest();
            request.Participants = Enumerable.Empty<ParticipantRequest>().ToList();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(2);
            result.Errors
                .Any(x => x.ErrorMessage == BookNewConferenceRequestValidation.NoParticipantsErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_participants_error()
        {
            var request = BuildRequest();
            request.Participants[0].Username = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == ParticipantRequestValidation.NoUsernameErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_missing_service_id_error()
        {
            var request = BuildRequest();
            request.CaseTypeServiceId = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == BookNewConferenceRequestValidation.NoCaseTypeServiceIdErrorMessage)
                .Should().BeTrue();
        }
    }
}
