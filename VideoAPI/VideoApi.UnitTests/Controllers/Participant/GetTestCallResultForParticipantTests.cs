using System;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Video.API.Controllers;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Participant
{
    public class GetTestCallResultForParticipantTests
    {
        private ParticipantsController _controller;
        private Mock<IQueryHandler> _mockQueryHandler;
        private Mock<ICommandHandler> _mockCommandHandler;
        private Mock<IVideoPlatformService> _mockVideoPlatformService;
        private Mock<ILogger<ParticipantsController>> _mockLogger;
        
        [SetUp]
        public void Setup()
        {
            _mockQueryHandler = new Mock<IQueryHandler>();
            _mockCommandHandler = new Mock<ICommandHandler>();
            _mockVideoPlatformService = new Mock<IVideoPlatformService>();
            _mockLogger = new Mock<ILogger<ParticipantsController>>();
            
            _controller = new ParticipantsController(_mockCommandHandler.Object, _mockQueryHandler.Object,
                _mockVideoPlatformService.Object, _mockLogger.Object);
        }

        [Test]
        public async Task should_return_okay_with_response()
        {
            var testResult = Builder<TestCallResult>.CreateNew()
                .WithFactory(() => new TestCallResult(true, TestScore.Good)).Build();
            
            _mockVideoPlatformService
                .Setup(x => x.GetTestCallScoreAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(testResult));

            var conferenceId = Guid.NewGuid();
            var participantId = Guid.NewGuid();
            var command =
                new UpdateSelfTestCallResultCommand(conferenceId, participantId, testResult.Passed, testResult.Score);
            _mockCommandHandler.Setup(x => x.Handle(command));
            
            var response = await _controller.GetTestCallResultForParticipant(Guid.NewGuid(), Guid.NewGuid());
            var typedResult = (OkObjectResult) response;
            typedResult.Should().NotBeNull();
        }

        [Test]
        public async Task should_return_not_found()
        {
            _mockVideoPlatformService
                .Setup(x => x.GetTestCallScoreAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult<TestCallResult>(null));
            
            var response = await _controller.GetTestCallResultForParticipant(Guid.NewGuid(), Guid.NewGuid());
            var typedResult = (NotFoundResult) response;
            typedResult.Should().NotBeNull();

            _mockCommandHandler.Verify(x => x.Handle(It.IsAny<UpdateSelfTestCallResultCommand>()), Times.Never);
        }

        [Test]
        public async Task should_update_test_score_to_database()
        {
            var testResult = new UpdateSelfTestScoreRequest() {  Passed = true, Score = TestScore.Good};

            var conferenceId = Guid.NewGuid();
            var participantId = Guid.NewGuid();

            var response = await _controller.UpdateSelfTestScore(Guid.NewGuid(), Guid.NewGuid(), testResult);
            var typedResult = (NoContentResult)response;
            typedResult.Should().NotBeNull();
        }
    }
}