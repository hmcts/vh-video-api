using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Video.API.Validations;
using VideoApi.Contract.Requests;

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
        public async Task should_pass_validation()
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
        public async Task should_fail_validation_when_participant_id_is_invalid()
        {
            var request = new TransferParticipantRequest
            {
                ParticipantId = Guid.Empty,
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
