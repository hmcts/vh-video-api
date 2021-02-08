using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.Contract.Requests;
using VideoApi.Validations;

namespace VideoApi.UnitTests.Validation
{
    public class AddTaskRequestValidationTests
    {
        private AddTaskRequestValidation _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new AddTaskRequestValidation();
        }

        [Test]
        public async Task Should_pass_validation()
        {
            var request = new AddTaskRequest { ParticipantId = Guid.NewGuid(), TaskType = VideoApi.Domain.Enums.TaskType.Participant, Body = "alert name" };
            var result = await _validator.ValidateAsync(request);
            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_fail_validation_if_participant_is_empty()
        {
            var request = new AddTaskRequest { ParticipantId = Guid.Empty, TaskType = VideoApi.Domain.Enums.TaskType.Participant, Body = "alert name" };
            var result = await _validator.ValidateAsync(request);
            result.IsValid.Should().BeFalse();
        }

        [Test]
        public async Task Should_fail_validation_if_body_is_empty()
        {
            var request = new AddTaskRequest { TaskType = VideoApi.Domain.Enums.TaskType.Participant, Body = "" };
            var result = await _validator.ValidateAsync(request);
            result.IsValid.Should().BeFalse();
        }
    }
}