using System;
using System.Threading.Tasks;
using VideoApi.Contract.Requests;
using VideoApi.Validations;

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
        public async Task Should_pass_validation()
        {
            var request = BuildRequest();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_fail_validation_when_conference_is_empty()
        {
            var request = BuildRequest();
            request.ConferenceId = Guid.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Exists(x => x.ErrorMessage == ConsultationRequestValidation.NoConferenceIdErrorMessage)
                .Should().BeTrue();
        }


        [Test]
        public async Task Should_fail_validation_when_requested_for_is_empty()
        {
            var request = BuildRequest();
            request.RequestedFor = Guid.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Exists(x => x.ErrorMessage == ConsultationRequestValidation.NoRequestedForIdErrorMessage)
                .Should().BeTrue();
        }

        private static ConsultationRequestResponse BuildRequest()
        {
            return new ConsultationRequestResponse
            {
                ConferenceId = Guid.NewGuid(),
                RequestedBy = Guid.NewGuid(),
                RequestedFor = Guid.NewGuid(),
                Answer = ConsultationAnswer.None
            };
        }
    }
}
