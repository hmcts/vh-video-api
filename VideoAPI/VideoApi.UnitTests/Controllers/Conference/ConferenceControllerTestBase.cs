using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using Video.API.Controllers;
using VideoApi.Common.Configuration;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Conference
{
    public class ConferenceControllerTestBase
    {
        protected Mock<ICommandHandler> CommandHandlerMock;
        protected ConferenceController Controller;
        protected Mock<IQueryHandler> QueryHandlerMock;
        protected Mock<ILogger<ConferenceController>> MockLogger;
        protected Mock<IVideoPlatformService> VideoPlatformServiceMock;
        protected Mock<IOptions<ServicesConfiguration>> ServicesConfiguration;
        protected MeetingRoom MeetingRoom;
        protected VideoApi.Domain.Conference TestConference;

        [SetUp]
        public void Setup()
        {
            QueryHandlerMock = new Mock<IQueryHandler>();
            CommandHandlerMock = new Mock<ICommandHandler>();
            MockLogger = new Mock<ILogger<ConferenceController>>();
            VideoPlatformServiceMock = new Mock<IVideoPlatformService>();
            ServicesConfiguration = new Mock<IOptions<ServicesConfiguration>>();

            TestConference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Claimant", null, RoomType.ConsultationRoom1)
                .WithParticipant(UserRole.Representative, "Claimant")
                .WithParticipant(UserRole.Individual, "Defendant")
                .WithParticipant(UserRole.Representative, "Defendant")
                .Build();

            QueryHandlerMock
                .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(TestConference);

            QueryHandlerMock
                .Setup(x => x.Handle<GetConferenceByHearingRefIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByHearingRefIdQuery>()))
                .ReturnsAsync(TestConference);

            CommandHandlerMock
                .Setup(x => x.Handle(It.IsAny<SaveEventCommand>()))
                .Returns(Task.FromResult(default(object)));

            ServicesConfiguration.Setup(s => s.Value).Returns(new ServicesConfiguration());

            MeetingRoom = new MeetingRoom($"http://adminuri", $"http://judgeuri", $"http://participanturi", "pexipnode");

            Controller = new ConferenceController(QueryHandlerMock.Object, CommandHandlerMock.Object,
                 VideoPlatformServiceMock.Object, ServicesConfiguration.Object, MockLogger.Object);
        }
    }
}
