
using Microsoft.Extensions.Logging;
using Moq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Contract.Enums;
using VideoApi.Controllers;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
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
        private Mock<ISupplierPlatformServiceFactory> _mockSupplierPlatformServiceFactory;
        protected VideoApi.Domain.Conference TestConference;

        [SetUp]
        public void Setup()
        {
            MockQueryHandler = new Mock<IQueryHandler>();
            MockCommandHandler = new Mock<ICommandHandler>();
            MockVideoPlatformService = new Mock<IVideoPlatformService>();
            _mockSupplierPlatformServiceFactory = new Mock<ISupplierPlatformServiceFactory>();
            _mockSupplierPlatformServiceFactory.Setup(x => x.Create(It.IsAny<VideoApi.Domain.Enums.Supplier>())).Returns(MockVideoPlatformService.Object);
            
            _mockLogger = new Mock<ILogger<ParticipantsController>>();

            TestConference = new ConferenceBuilder()
              .WithParticipant(UserRole.Judge, null)
              .WithParticipant(UserRole.Individual, "Applicant", null, null, RoomType.ConsultationRoom)
              .WithParticipant(UserRole.Representative, "Applicant")
              .WithParticipant(UserRole.Individual, "Respondent")
              .WithParticipant(UserRole.Representative, "Respondent")
              .WithInterpreterRoom()
              .Build();

            MockQueryHandler
                .Setup(x =>
                    x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(TestConference);
            
            MockQueryHandler
                .Setup(x =>
                    x.Handle<GetConferenceForParticipantQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceForParticipantQuery>()))
                .ReturnsAsync(TestConference);
            
            Controller = new ParticipantsController(MockCommandHandler.Object, MockQueryHandler.Object,
                _mockSupplierPlatformServiceFactory.Object, _mockLogger.Object);
        }

        protected void VerifySupplierUsed(VideoApi.Domain.Enums.Supplier supplier, Times times)
        {
            _mockSupplierPlatformServiceFactory.Verify(x => x.Create(supplier), times);
        }
    }
}
