using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using Video.API.Controllers;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Consultation
{
    public abstract class ConsultationControllerTestBase
    {
        protected Mock<ICommandHandler> CommandHandlerMock;
        protected ConsultationController Controller;
        protected Mock<IQueryHandler> QueryHandlerMock;
        protected Mock<ILogger<ConsultationController>> MockLogger;
        protected Mock<IVideoPlatformService> VideoPlatformServiceMock;

        protected Conference TestConference;

        [SetUp]
        public void Setup()
        {
            QueryHandlerMock = new Mock<IQueryHandler>();
            CommandHandlerMock = new Mock<ICommandHandler>();
            MockLogger = new Mock<ILogger<ConsultationController>>();
            VideoPlatformServiceMock = new Mock<IVideoPlatformService>();

            TestConference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Claimant",null, RoomType.ConsultationRoom1)
                .WithParticipant(UserRole.Representative, "Claimant")
                .WithParticipant(UserRole.Individual, "Defendant")
                .WithParticipant(UserRole.Representative, "Defendant")
                .Build();

            QueryHandlerMock
                .Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(TestConference);

            CommandHandlerMock
                .Setup(x => x.Handle(It.IsAny<SaveEventCommand>()))
                .Returns(Task.FromResult(default(object)));

            Controller = new ConsultationController(QueryHandlerMock.Object, CommandHandlerMock.Object,
                MockLogger.Object, VideoPlatformServiceMock.Object);
        }
    }
}
