using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.Contract.Requests;
using VideoApi.Validations;

namespace VideoApi.UnitTests.Validation
{
    public class TransferParticipantRequestValidationTests
    {
        private TransferParticipantRequestValidation _validator;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new TransferParticipantRequestValidation();
        }

        [Test]
        public async Task should_pass_validation_when_participant_id_is_set()
        {
            var request = new TransferParticipantRequest
            {
                ParticipantId = Guid.NewGuid(),
                TransferType = TransferType.Call
            };
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
        
        [Test]
        public async Task should_pass_validation_when_room_id_is_set()
        {
            var request = new TransferParticipantRequest
            {
                RoomId = 1027,
                TransferType = TransferType.Call
            };
            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
        
        [Test]
        public async Task should_fail_validation_when_participant_id_is_invalid_and_room_id_is_null()
        {
            var request = new TransferParticipantRequest
            {
                ParticipantId = Guid.Empty,
                RoomId = null,
                TransferType = TransferType.Call
            };
            var result = await _validator.ValidateAsync(request);
            result.Errors.Any(x => x.ErrorMessage == TransferParticipantRequestValidation.MissingParticipantId)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task should_fail_validation_when_room_id_is_negative_and_participant_id_is_null()
        {
            var request = new TransferParticipantRequest
            {
                ParticipantId = null,
                RoomId = -3837,
                TransferType = TransferType.Call
            };
            var result = await _validator.ValidateAsync(request);
            result.Errors.Any(x => x.ErrorMessage == TransferParticipantRequestValidation.MissingParticipantId)
                .Should().BeTrue();
        }
        
        [Test]
        public async Task should_fail_validation_when_transfer_type_is_invalid()
        {
            var request = new TransferParticipantRequest
            {
                ParticipantId = Guid.NewGuid(),
                TransferType = null
            };
            var result = await _validator.ValidateAsync(request);
            result.Errors.Any(x => x.ErrorMessage == TransferParticipantRequestValidation.MissingTransferType)
                .Should().BeTrue();
        }
    }
}
