
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using Video.API.Controllers;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Services;

namespace VideoApi.UnitTests.Controllers.Participant
{
    public class ParticipantsControllerTestBase
    {
        protected ParticipantsController _controller;
        protected Mock<IQueryHandler> _mockQueryHandler;
        protected Mock<ICommandHandler> _mockCommandHandler;
        protected Mock<IVideoPlatformService> _mockVideoPlatformService;
        protected Mock<ILogger<ParticipantsController>> _mockLogger;
        protected VideoApi.Domain.Conference TestConference;

        [SetUp]
        public void Setup()
        {
            _mockQueryHandler = new Mock<IQueryHandler>();
            _mockCommandHandler = new Mock<ICommandHandler>();
            _mockVideoPlatformService = new Mock<IVideoPlatformService>();
            _mockLogger = new Mock<ILogger<ParticipantsController>>();

            TestConference = new ConferenceBuilder()
              .WithParticipant(UserRole.Judge, null)
              .WithParticipant(UserRole.Individual, "Claimant", null, RoomType.ConsultationRoom1)
              .WithParticipant(UserRole.Representative, "Claimant")
              .WithParticipant(UserRole.Individual, "Defendant")
              .WithParticipant(UserRole.Representative, "Defendant")
              .Build();

            _controller = new ParticipantsController(_mockCommandHandler.Object, _mockQueryHandler.Object,
                _mockVideoPlatformService.Object, _mockLogger.Object);
        }
    }
}
