using System;
using Autofac.Extras.Moq;
using Microsoft.Extensions.Logging;
using Moq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Contract.Enums;
using VideoApi.Controllers;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Services;
using VideoApi.Services.Contracts;
using ParticipantState = VideoApi.Domain.Enums.ParticipantState;
using RoomType = VideoApi.Domain.Enums.RoomType;
using UserRole = VideoApi.Domain.Enums.UserRole;

namespace VideoApi.UnitTests.Controllers.ConferenceManagement
{
    public class ConferenceManagementControllerTestBase
    {
        private Mock<IQueryHandler> _queryHandlerMock;
        private Mock<ISupplierPlatformServiceFactory> _supplierPlatformServiceFactory;
        protected ConferenceManagementController Controller;
        protected AutoMock Mocker;
        protected Mock<ILogger<ConferenceManagementController>> MockLogger;
        protected VideoApi.Domain.Conference TestConference;
        protected Mock<IVideoPlatformService> VideoPlatformServiceMock;
        
        [SetUp]
        public void Setup()
        {
            TestConference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Applicant", null, null, RoomType.ConsultationRoom)
                .WithParticipant(UserRole.Representative, "Applicant", "rep1@hmcts.net")
                .WithParticipant(UserRole.Individual, "Respondent")
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithEndpoint("Endpoint With DA", $"{Guid.NewGuid():N}@hmcts.net", true)
                .WithEndpoint("Endpoint Without DA", $"{Guid.NewGuid():N}@hmcts.net")
                .Build();
            Mocker = AutoMock.GetLoose();
            MockLogger = Mocker.Mock<ILogger<ConferenceManagementController>>();

            UpdateConferenceQueryMock();
            VideoPlatformServiceMock = Mocker.Mock<IVideoPlatformService>();
            _supplierPlatformServiceFactory = Mocker.Mock<ISupplierPlatformServiceFactory>();
            _supplierPlatformServiceFactory.Setup(x => x.Create(It.IsAny<VideoApi.Domain.Enums.Supplier>())).Returns(VideoPlatformServiceMock.Object);

            _queryHandlerMock = Mocker.Mock<IQueryHandler>();
            _queryHandlerMock
                .Setup(x =>
                    x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(TestConference);
            
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
                "displayName", "userName") { HearingRole = "Witness", UserRole = UserRole.Individual, State = ParticipantState.Available });
        }
        
        protected void AddTelephoneParticipantToTestConference()
        {
            TestConference.AddTelephoneParticipant(new TelephoneParticipant(Guid.NewGuid(), "Anonymous", TestConference));
        }
        
        protected void AddQuicklinkToTestConference()
        {
            TestConference.AddParticipant(new VideoApi.Domain.QuickLinkParticipant("QuciklinkName", UserRole.QuickLinkParticipant) {  State = ParticipantState.Available});
        }
        
        protected void VerifySupplierUsed(Supplier supplier, Times times)
        {
            VerifySupplierUsed((VideoApi.Domain.Enums.Supplier)supplier, times);
        }
        
        protected void VerifySupplierUsed(VideoApi.Domain.Enums.Supplier supplier, Times times)
        {
            _supplierPlatformServiceFactory.Verify(x => x.Create(supplier), times);
        }
    }
}
