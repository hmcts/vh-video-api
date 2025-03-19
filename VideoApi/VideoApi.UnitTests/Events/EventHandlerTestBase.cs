using Autofac.Extras.Moq;
using Moq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services;
using VideoApi.Services.Contracts;

namespace VideoApi.UnitTests.Events
{
    public abstract class EventHandlerTestBase<TSystemUnderTest>
    {
        protected AutoMock _mocker;
        private Mock<ISupplierPlatformServiceFactory> _supplierPlatformServiceFactoryMock;
        protected TSystemUnderTest _sut;
        
        protected Mock<ICommandHandler> CommandHandlerMock;
        protected Mock<IQueryHandler> QueryHandlerMock;
        
        protected Conference TestConference;
        
        protected Mock<IVideoPlatformService> VideoPlatformServiceMock;
        
        [SetUp]
        public void Setup()
        {
            _mocker = AutoMock.GetLoose();
            _sut = _mocker.Create<TSystemUnderTest>();

            QueryHandlerMock = _mocker.Mock<IQueryHandler>();
            CommandHandlerMock = _mocker.Mock<ICommandHandler>();

            TestConference = new ConferenceBuilder()
                .WithEndpoint("Endpoint1", "Endpoint1234@sip.com")
                .WithEndpoint("Endpoint2", "Endpoint2345@sip.com")
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Applicant")
                .WithParticipant(UserRole.Representative, "Applicant")
                .WithParticipant(UserRole.Individual, "Respondent")
                .WithParticipant(UserRole.Representative, "Respondent", "DA1@test.com")
                .WithLinkedParticipant(UserRole.Individual, "Applicant")
                .WithInterpreterRoom()
                .WithTelephoneParticipant("+44123456789")
                .Build();

            QueryHandlerMock
                .Setup(x => x.Handle<GetConferenceByIdForEventQuery, Conference>(It.IsAny<GetConferenceByIdForEventQuery>()))
                .ReturnsAsync(TestConference);
            
            VideoPlatformServiceMock = _mocker.Mock<IVideoPlatformService>();
            _supplierPlatformServiceFactoryMock = _mocker.Mock<ISupplierPlatformServiceFactory>();
            _supplierPlatformServiceFactoryMock.Setup(x => x.Create(It.IsAny<Supplier>())).Returns(VideoPlatformServiceMock.Object);    
        }
        
        protected void VerifySupplierUsed(Supplier supplier, Times times)
        {
            _supplierPlatformServiceFactoryMock.Verify(x => x.Create(supplier), times);
        }
    }
}
