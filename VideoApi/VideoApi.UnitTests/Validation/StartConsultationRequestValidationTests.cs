using System;
using System.Threading.Tasks;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Enums;
using VideoApi.Validations;

namespace VideoApi.UnitTests.Validation
{
    public class StartConsultationRequestValidationTests
    {
        private StartConsultationRequestValidation _validator;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new StartConsultationRequestValidation();
        }

        [Test]
        public async Task should_pass_validation()
        {
            var request = new StartConsultationRequest
            {
                ConferenceId = Guid.NewGuid(),
                RequestedBy = Guid.NewGuid(),
                RoomType = VirtualCourtRoomType.JudgeJOH
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
        
        [Test]
        public async Task should_fail_validation_when_values_are_invalid()
        {
            var request = new StartConsultationRequest
            {
                ConferenceId = Guid.Empty,
                RequestedBy = Guid.Empty,
                RoomType = (VirtualCourtRoomType)20
            };
            
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
        }
    }
}
