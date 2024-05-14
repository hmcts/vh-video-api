using System;
using Autofac.Extras.Moq;
using Microsoft.Extensions.Logging;
using Moq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Controllers;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Services.Contracts;

namespace VideoApi.UnitTests.Controllers.ConferenceManagement
{
    public class ConferenceManagementControllerTestBase
    {
        protected ConferenceManagementController Controller;
        protected Mock<ILogger<ConferenceManagementController>> MockLogger;
        protected Mock<IVideoPlatformService> VideoPlatformServiceMock;
        protected AutoMock Mocker;
        protected VideoApi.Domain.Conference TestConference;

        [SetUp]
        public void Setup()
        {
            TestConference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Applicant", null, null, RoomType.ConsultationRoom)
                .WithParticipant(UserRole.Representative, "Applicant", "rep1@hmcts.net")
                .WithParticipant(UserRole.Individual, "Respondent")
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithEndpoint("Endpoint With DA", $"{Guid.NewGuid():N}@hmcts.net", "rep1@hmcts.net")
                .WithEndpoint("Endpoint Without DA", $"{Guid.NewGuid():N}@hmcts.net")
                .Build();
            Mocker = AutoMock.GetLoose();
            MockLogger = Mocker.Mock<ILogger<ConferenceManagementController>>();

            UpdateConferenceQueryMock();
            VideoPlatformServiceMock = Mocker.Mock<IVideoPlatformService>();

            Controller = Mocker.Create<ConferenceManagementController>();
        }

        protected void UpdateConferenceQueryMock()
        {
            Mocker.Mock<IQueryHandler>()
                .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(
                    It.Is<GetConferenceByIdQuery>(q => q.ConferenceId == TestConference.Id)))
                .ReturnsAsync(TestConference);
        }
        
        protected void AddWitnessToTestConference()
        {
            TestConference.AddParticipant(new VideoApi.Domain.Participant(Guid.NewGuid(), "contactEmail", 
                "telephone", "displayName", "firstName", "lastName", "name", "userName") { HearingRole = "Witness", UserRole = UserRole.Individual, State = ParticipantState.Available });
        }

        protected void AddQuicklinkToTestConference()
        {
            TestConference.AddParticipant(new VideoApi.Domain.QuickLinkParticipant("QuciklinkName", UserRole.QuickLinkParticipant) {  State = ParticipantState.Available});
        }
    }
}
