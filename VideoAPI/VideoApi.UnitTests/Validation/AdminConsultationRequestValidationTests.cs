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
    public class AdminConsultationRequestValidationTests
    {
        private AdminConsultationRequestValidation _validator;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new AdminConsultationRequestValidation();
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
            result.Errors.Any(x => x.ErrorMessage == AdminConsultationRequestValidation.NoConferenceIdErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task should_fail_validation_when_participant_id_is_empty()
        {
            var request = BuildRequest();
            request.ParticipantId = Guid.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Any(x => x.ErrorMessage == AdminConsultationRequestValidation.NoParticipantIdErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task should_fail_validation_when_answer_is_invalid()
        {
            var request = BuildRequest();
            request.Answer = ConsultationAnswer.None;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Any(x => x.ErrorMessage == AdminConsultationRequestValidation.NoAnswerErrorMessage)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task should_fail_validation_when_room_is_not_consultation()
        {
            var request = BuildRequest();
            request.ConsultationRoom = RoomType.AdminRoom;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Any(x => x.PropertyName == "ConsultationRoom")
                .Should().BeTrue();
            result.Errors.Any(x => x.ErrorMessage == AdminConsultationRequestValidation.NotValidConsultationRoomMessage)
                .Should().BeTrue();
        }
        
        private AdminConsultationRequest BuildRequest()
        {
            var request = Builder<AdminConsultationRequest>.CreateNew()
                .With(x => x.ConferenceId = Guid.NewGuid())
                .With(x => x.ParticipantId = Guid.NewGuid())
                .With(x => x.Answer = ConsultationAnswer.Accepted)
                .With(x => x.ConsultationRoom = RoomType.ConsultationRoom1)
                .Build();
            return request;
        }
    }
}
