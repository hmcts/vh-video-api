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
    public class AddParticipantsToConferenceRequestValidationTests
    {
        private AddParticipantsToConferenceRequestValidation _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new AddParticipantsToConferenceRequestValidation();
        }

        [Test]
        public async Task Should_pass_validation()
        {
            var request = BuildRequest();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_return_missing_participants_error()
        {
            var request = BuildRequest();
            request.Participants = Enumerable.Empty<ParticipantRequest>().ToList();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(2);
            result.Errors
                .Any(x => x.ErrorMessage == AddParticipantsToConferenceRequestValidation.NoParticipantsErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_participants_error()
        {
            var request = BuildRequest();
            request.Participants[0].Name = string.Empty;

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == ParticipantRequestValidation.NoNameErrorMessage)
                .Should().BeTrue();
        }

        private AddParticipantsToConferenceRequest BuildRequest()
        {
            var participants = Builder<ParticipantRequest>.CreateListOfSize(4)
                .All().With(x => x.UserRole = UserRole.Individual).Build().ToList();

            return new AddParticipantsToConferenceRequest
            {
                Participants = participants
            };
        }
    }
}