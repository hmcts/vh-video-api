using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Video.API.Controllers;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;
using VideoApi.Services;

namespace VideoApi.UnitTests.Controllers.Participant
{
    public class GetTestCallResultForParticipantTests
    {
        private ParticipantsController _controller;
        private Mock<IQueryHandler> _mockQueryHandler;
        private Mock<ICommandHandler> _mockCommandHandler;
        private Mock<IVideoPlatformService> _mockVideoPlatformService;
        
        [SetUp]
        public void Setup()
        {
            _mockQueryHandler = new Mock<IQueryHandler>();
            _mockCommandHandler = new Mock<ICommandHandler>();
            _mockVideoPlatformService = new Mock<IVideoPlatformService>();

            _controller = new ParticipantsController(_mockCommandHandler.Object, _mockQueryHandler.Object,
                _mockVideoPlatformService.Object);
        }

        [Test]
        public async Task should_return_okay_with_response()
        {
            var testResult = new TestCallResult
            {
                Passed = true,
                Score = TestScore.Good
            };
            
            _mockVideoPlatformService
                .Setup(x => x.GetTestCallScoreAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(testResult));

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
        }
    }
}