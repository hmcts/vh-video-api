using System;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Participant
{
    public class GetIndependentTestCallResult : ParticipantsControllerTestBase
    {

        [Test]
        public async Task should_return_okay_with_response()
        {
            var testResult = Builder<TestCallResult>.CreateNew()
                .WithFactory(() => new TestCallResult(true, TestScore.Good)).Build();

            _mockVideoPlatformService
                .Setup(x => x.GetTestCallScoreAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(testResult));

            var participantId = Guid.NewGuid();

            var response = await _controller.GetIndependentTestCallResult(participantId);
            var typedResult = (OkObjectResult)response;
            typedResult.Should().NotBeNull();
        }

        [Test]
        public async Task should_return_not_found()
        {
            _mockVideoPlatformService
                .Setup(x => x.GetTestCallScoreAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult<TestCallResult>(null));

            var response = await _controller.GetIndependentTestCallResult(Guid.NewGuid());
            var typedResult = (NotFoundResult)response;
            typedResult.Should().NotBeNull();
        }
    }
}
