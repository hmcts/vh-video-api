using System;
using Moq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Services.Consultation
{
    public abstract class ConsultationServiceTestBase
    {
        protected Mock<ICommandHandler> CommandHandlerMock;
        protected Mock<IQueryHandler> QueryHandlerMock;

        protected VideoApi.Domain.Conference TestConference;

        protected void SetupTestConference()
        {
            QueryHandlerMock = new Mock<IQueryHandler>();
            CommandHandlerMock = new Mock<ICommandHandler>();
            
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
        }
    }
}
