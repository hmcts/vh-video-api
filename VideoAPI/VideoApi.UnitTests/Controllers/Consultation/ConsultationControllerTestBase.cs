using System;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using Video.API.Controllers;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Services.Contracts;
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
        protected Mock<IConsultationService> ConsultationServiceMock;

        protected VideoApi.Domain.Conference TestConference;

        [SetUp]
        public void Setup()
        {
            QueryHandlerMock = new Mock<IQueryHandler>();
            CommandHandlerMock = new Mock<ICommandHandler>();
            MockLogger = new Mock<ILogger<ConsultationController>>();
            VideoPlatformServiceMock = new Mock<IVideoPlatformService>();
            ConsultationServiceMock = new Mock<IConsultationService>();

            TestConference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Claimant", null, null, RoomType.ConsultationRoom1)
                .WithParticipant(UserRole.Representative, "Claimant", "rep1@dA.com")
                .WithParticipant(UserRole.Individual, "Defendant")
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithEndpoint("Endpoint With DA", $"{Guid.NewGuid():N}@test.hearings.com", "rep1@dA.com")
                .WithEndpoint("Endpoint Without DA", $"{Guid.NewGuid():N}@test.hearings.com")
                .Build();

            QueryHandlerMock
                .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(
                    It.Is<GetConferenceByIdQuery>(q => q.ConferenceId == TestConference.Id)))
                .ReturnsAsync(TestConference);

            CommandHandlerMock
                .Setup(x => x.Handle(It.IsAny<SaveEventCommand>()))
                .Returns(Task.FromResult(default(object)));

            Controller = new ConsultationController(QueryHandlerMock.Object, CommandHandlerMock.Object,
                MockLogger.Object, VideoPlatformServiceMock.Object, ConsultationServiceMock.Object);
        }
    }
}
