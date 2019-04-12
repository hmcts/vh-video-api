using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Video.API.Validations;
using VideoApi.Contract.Requests;

namespace VideoApi.UnitTests.Validation
{
    public class ConsultationRequestValidationTests
    {
        private ConsultationRequestValidation _validator;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new ConsultationRequestValidation();
        }
        
        [Test]
        public async Task should_pass_validation()
        {
            var request = BuildRequest();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
        
        [Test]
        public async Task should_fail_validation_when_conference_is_empty()
        {
            var request = BuildRequest();
            request.ConferenceId = Guid.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Any(x => x.ErrorMessage == ConsultationRequestValidation.NoConferenceIdErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task should_fail_validation_when_requested_by_is_empty()
        {
            var request = BuildRequest();
            request.RequestedBy = Guid.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Any(x => x.ErrorMessage == ConsultationRequestValidation.NoRequestedByIdErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task should_fail_validation_when_requested_for_is_empty()
        {
            var request = BuildRequest();
            request.RequestedFor = Guid.Empty;
            
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Any(x => x.ErrorMessage == ConsultationRequestValidation.NoRequestedForIdErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task should_fail_validation_when_answer_is_invalid()
        {
            var request = BuildRequest();
            request.Answer = ConsultationAnswer.None;
            
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Any(x => x.ErrorMessage == ConsultationRequestValidation.NoAnswerErrorMessage)
                .Should().BeTrue();
        }

        private ConsultationRequest BuildRequest()
        {
            return new ConsultationRequest
            {
                ConferenceId = Guid.NewGuid(),
                RequestedBy = Guid.NewGuid(),
                RequestedFor = Guid.NewGuid(),
                Answer = null
            };
        }
    }
}