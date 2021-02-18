using System;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.Contract.Requests;
using VideoApi.Validations;

namespace VideoApi.UnitTests.Validation
{
    public class ConsultationRequestResponseValidationTests
    {
        private ConsultationRequestResponseValidation _validator;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new ConsultationRequestResponseValidation();
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
            result.Errors.Any(x => x.ErrorMessage == ConsultationRequestResponseValidation.NoConferenceIdErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_fail_validation_when_participant_id_is_empty()
        {
            var request = BuildRequest();
            request.RequestedFor = Guid.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Any(x => x.ErrorMessage == ConsultationRequestResponseValidation.NoParticipantIdErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task Should_fail_validation_when_answer_is_invalid()
        {
            var request = BuildRequest();
            request.Answer = ConsultationAnswer.None;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Any(x => x.ErrorMessage == ConsultationRequestResponseValidation.NoAnswerErrorMessage)
                .Should().BeTrue();
        }
        
        private ConsultationRequestResponse BuildRequest()
        {
            var request = Builder<ConsultationRequestResponse>.CreateNew()
                .With(x => x.ConferenceId = Guid.NewGuid())
                .With(x => x.RequestedFor = Guid.NewGuid())
                .With(x => x.Answer = ConsultationAnswer.Accepted)
                .With(x => x.RoomLabel = "ConsultationRoom")
                .Build();
            return request;
        }
    }
}
