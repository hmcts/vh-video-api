using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.Contract.Requests;
using VideoApi.Validations;

namespace VideoApi.UnitTests.Validation
{
    public class UpdateConferenceParticipantsRequestValidationTests
    {
        private readonly UpdateConferenceParticipantsRequestValidation _validator = 
            new UpdateConferenceParticipantsRequestValidation();

        [Test]
        public async Task Should_pass_validation()
        {
            //Arrange
            var request = BuildRequest();

            //Act
            var result = await _validator.ValidateAsync(request);

            //Assert
            result.IsValid.Should().BeTrue();
        }

    
        [Test]
        public async Task Should_return_error()
        {
            //Arrange
            var request = new UpdateConferenceParticipantsRequest
            {
                ExistingParticipants = new List<UpdateParticipantRequest>(),
                LinkedParticipants = new List<LinkedParticipantRequest>(),
                NewParticipants = new List<ParticipantRequest>(),
                RemovedParticipants = new List<Guid>()
            };

            //Act
            var result = await _validator.ValidateAsync(request);

            //Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == UpdateConferenceParticipantsRequestValidation.NoParticipantsErrorMessage);
        }

        private UpdateConferenceParticipantsRequest BuildRequest()
        {
            return new UpdateConferenceParticipantsRequest
            {
                ExistingParticipants = new List<UpdateParticipantRequest> { new UpdateParticipantRequest() },
                LinkedParticipants = new List<LinkedParticipantRequest> { new LinkedParticipantRequest() },
                NewParticipants = new List<ParticipantRequest> { new ParticipantRequest() },
                RemovedParticipants = new List<Guid> { Guid.NewGuid() }
            };
        }
    }
}
