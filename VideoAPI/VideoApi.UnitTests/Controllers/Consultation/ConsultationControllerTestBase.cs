using Microsoft.AspNetCore.SignalR;
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
using VideoApi.Events.Hub;
using VideoApi.Services;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Consultation
{
    public abstract class ConsultationControllerTestBase
    {
        protected Mock<ICommandHandler> CommandHandlerMock;
        protected ConsultationController Controller;
        protected Mock<IEventHubClient> EventHubClientMock;
        protected Mock<IHubContext<EventHub, IEventHubClient>> HubContextMock;
        protected Mock<IQueryHandler> QueryHandlerMock;
        protected Mock<ILogger<ConsultationController>> MockLogger;
        protected Mock<IVideoPlatformService> VideoPlatformServiceMock;

        protected Conference TestConference;

        [SetUp]
        public void Setup()
        {
            QueryHandlerMock = new Mock<IQueryHandler>();
            CommandHandlerMock = new Mock<ICommandHandler>();
            HubContextMock = new Mock<IHubContext<EventHub, IEventHubClient>>();
            EventHubClientMock = new Mock<IEventHubClient>();
            MockLogger = new Mock<ILogger<ConsultationController>>();
            VideoPlatformServiceMock = new Mock<IVideoPlatformService>();

            TestConference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Claimant")
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
                HubContextMock.Object, MockLogger.Object, VideoPlatformServiceMock.Object);

            foreach (var participant in TestConference.GetParticipants())
            {
                HubContextMock.Setup(x => x.Clients.Group(participant.Username.ToString()))
                    .Returns(EventHubClientMock.Object);
            }

            HubContextMock.Setup(x => x.Clients.Group(EventHub.VhOfficersGroupName))
                .Returns(EventHubClientMock.Object);
        }
    }
}