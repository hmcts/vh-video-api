
using Microsoft.Extensions.Logging;
using Moq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Contract.Enums;
using VideoApi.Controllers;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Services;
using VideoApi.Services.Contracts;
using RoomType = VideoApi.Domain.Enums.RoomType;
using UserRole = VideoApi.Domain.Enums.UserRole;

namespace VideoApi.UnitTests.Controllers.Participant
{
    public class ParticipantsControllerTestBase
    {
        protected ParticipantsController Controller;
        protected Mock<IQueryHandler> MockQueryHandler;
        protected Mock<ICommandHandler> MockCommandHandler;
        protected Mock<IVideoPlatformService> MockVideoPlatformService;
        protected Mock<ILogger<ParticipantsController>> _mockLogger;
        protected VideoApi.Domain.Conference TestConference;

        [SetUp]
        public void Setup()
        {
            MockQueryHandler = new Mock<IQueryHandler>();
            MockCommandHandler = new Mock<ICommandHandler>();
            MockVideoPlatformService = new Mock<IVideoPlatformService>();
            var supplierPlatformServiceFactory = new Mock<ISupplierPlatformServiceFactory>();
            supplierPlatformServiceFactory.Setup(x => x.Create(It.IsAny<Supplier>())).Returns(MockVideoPlatformService.Object);
            
            _mockLogger = new Mock<ILogger<ParticipantsController>>();

            TestConference = new ConferenceBuilder()
              .WithParticipant(UserRole.Judge, null)
              .WithParticipant(UserRole.Individual, "Applicant", null, null, RoomType.ConsultationRoom)
              .WithParticipant(UserRole.Representative, "Applicant")
              .WithParticipant(UserRole.Individual, "Respondent")
              .WithParticipant(UserRole.Representative, "Respondent")
              .WithInterpreterRoom()
              .Build();

            Controller = new ParticipantsController(MockCommandHandler.Object, MockQueryHandler.Object,
                supplierPlatformServiceFactory.Object, _mockLogger.Object);
        }
    }
}
