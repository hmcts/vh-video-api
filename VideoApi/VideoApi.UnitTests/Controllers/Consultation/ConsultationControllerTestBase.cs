using System;
using Microsoft.Extensions.Logging;
using Moq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Controllers;
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
        protected Mock<IConsultationService> ConsultationServiceMock;
        protected ConsultationController Controller;
        protected Mock<ILogger<ConsultationController>> MockLogger;
        protected Mock<IQueryHandler> QueryHandlerMock;
        
        protected VideoApi.Domain.Conference TestConference;
        
        [SetUp]
        public void Setup()
        {
            QueryHandlerMock = new Mock<IQueryHandler>();
            CommandHandlerMock = new Mock<ICommandHandler>();
            MockLogger = new Mock<ILogger<ConsultationController>>();
            ConsultationServiceMock = new Mock<IConsultationService>();

            TestConference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Applicant", null, null, RoomType.ConsultationRoom)
                .WithParticipant(UserRole.Representative, "Applicant", "rep1@hmcts.net")
                .WithParticipant(UserRole.Individual, "Respondent")
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.StaffMember, null)
                .WithEndpoint("Endpoint 1", $"{Guid.NewGuid():N}@hmcts.net")
                .WithEndpoint("Endpoint 2", $"{Guid.NewGuid():N}@hmcts.net")
                .Build();

            QueryHandlerMock
                .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(
                    It.Is<GetConferenceByIdQuery>(q => q.ConferenceId == TestConference.Id)))
                .ReturnsAsync(TestConference);

            CommandHandlerMock
                .Setup(x => x.Handle(It.IsAny<SaveEventCommand>()))
                .Returns(Task.FromResult(default(object)));

            Controller = new ConsultationController(QueryHandlerMock.Object,
                MockLogger.Object, ConsultationServiceMock.Object, CommandHandlerMock.Object);
        }
    }
}
