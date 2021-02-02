using Autofac.Extras.Moq;
using Moq;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Events
{
    public abstract class EventHandlerTestBase<TSystemUnderTest>
    {
        protected TSystemUnderTest _sut;
        protected AutoMock _mocker;

        protected Mock<ICommandHandler> CommandHandlerMock;
        protected Mock<IQueryHandler> QueryHandlerMock;

        protected Conference TestConference;

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
                .WithParticipant(UserRole.Individual, "Claimant")
                .WithParticipant(UserRole.Representative, "Claimant")
                .WithParticipant(UserRole.Individual, "Defendant")
                .WithParticipant(UserRole.Representative, "Defendant")
                .Build();

            QueryHandlerMock
                .Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(TestConference);
        }
    }
}
