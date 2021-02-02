using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.Contract.Requests;
using VideoApi.Validations;

namespace VideoApi.UnitTests.Validation
{
    public class EndpointConsultationRequestValidationTests
    {
        private EndpointConsultationRequestValidation _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new EndpointConsultationRequestValidation();
        }

        [Test]
        public async Task Should_pass_validation()
        {
            var request = new EndpointConsultationRequest
            {
                ConferenceId = Guid.NewGuid(),
                EndpointId = Guid.NewGuid(),
                DefenceAdvocateId = Guid.NewGuid()
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_fail_validation_when_conference_id_empty()
        {
            var request = new EndpointConsultationRequest
            {
                ConferenceId = Guid.Empty,
                EndpointId = Guid.NewGuid(),
                DefenceAdvocateId = Guid.NewGuid()
            };

            var result = await _validator.ValidateAsync(request);
            result.Errors.Any(x => x.ErrorMessage == EndpointConsultationRequestValidation.NoConferenceError)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_fail_validation_endpoint_id_is_empty()
        {
            var request = new EndpointConsultationRequest
            {
                ConferenceId = Guid.NewGuid(),
                EndpointId = Guid.Empty,
                DefenceAdvocateId = Guid.NewGuid()
            };

            var result = await _validator.ValidateAsync(request);
            result.Errors.Any(x => x.ErrorMessage == EndpointConsultationRequestValidation.NoEndpointError)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_fail_validation_when_defence_advocate_id_is_empty()
        {
            var request = new EndpointConsultationRequest
            {
                ConferenceId = Guid.NewGuid(),
                EndpointId = Guid.NewGuid(),
                DefenceAdvocateId = Guid.Empty
            };

            var result = await _validator.ValidateAsync(request);
            result.Errors.Any(x => x.ErrorMessage == EndpointConsultationRequestValidation.NoDefenceAdvocateError)
                .Should().BeTrue();
        }
    }
}
