using System;
using System.Linq;
using System.Threading.Tasks;
using VideoApi.Contract.Requests;
using VideoApi.Validations;

namespace VideoApi.UnitTests.Validation
{
    public class LeaveConsultationRequestValidationTests
    {
        private LeaveConsultationRequestValidation _validator;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new LeaveConsultationRequestValidation();
        }
        
        [Test]
        public async Task Should_pass_validation()
        {
            var request = new LeaveConsultationRequest
            {
                ConferenceId = Guid.NewGuid(),
                ParticipantId = Guid.NewGuid()
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_missing_conference_id()
        {
            var request = new LeaveConsultationRequest
            {
                ParticipantId = Guid.NewGuid()
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors
                .Any(x => x.ErrorMessage == LeaveConsultationRequestValidation.NoConferenceIdErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_missing_participant_id()
        {
            var request = new LeaveConsultationRequest
            {
                ConferenceId = Guid.NewGuid()
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors
                .Any(x => x.ErrorMessage == LeaveConsultationRequestValidation.NoParticipantIdErrorMessage)
                .Should().BeTrue();
        }
        
    }
}
