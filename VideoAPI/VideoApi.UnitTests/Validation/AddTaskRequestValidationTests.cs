using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Video.API.Validations;
using VideoApi.Contract.Requests;

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
            var request = new AddTaskRequest { TaskType = VideoApi.Domain.Enums.TaskType.Participant, Body = "alert name" };
            var result = await _validator.ValidateAsync(request);
            result.IsValid.Should().BeTrue();
        }
    }
}
