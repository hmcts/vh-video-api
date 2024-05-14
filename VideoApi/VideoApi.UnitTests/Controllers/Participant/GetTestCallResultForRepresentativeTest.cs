using System;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Participant
{
    public class GetIndependentTestCallResult : ParticipantsControllerTestBase
    {

        [Test]
        public async Task Should_return_okay_with_response()
        {
            var testResult = Builder<TestCallResult>.CreateNew()
                .WithFactory(() => new TestCallResult(true, TestScore.Good)).Build();

            MockVideoPlatformService
                .Setup(x => x.GetTestCallScoreAsync(It.IsAny<Guid>(), It.IsAny<int>()))
                .Returns(Task.FromResult(testResult));

            var participantId = Guid.NewGuid();

            var response = await Controller.GetIndependentTestCallResultAsync(participantId);
            var typedResult = (OkObjectResult) response;
            typedResult.Should().NotBeNull();
        }

        [Test]
        public async Task Should_return_not_found()
        {
            MockVideoPlatformService
                .Setup(x => x.GetTestCallScoreAsync(It.IsAny<Guid>(), It.IsAny<int>()))
                .Returns(Task.FromResult<TestCallResult>(null));

            var response = await Controller.GetIndependentTestCallResultAsync(Guid.NewGuid());
            var typedResult = (NotFoundResult) response;
            typedResult.Should().NotBeNull();
        }
    }
}
