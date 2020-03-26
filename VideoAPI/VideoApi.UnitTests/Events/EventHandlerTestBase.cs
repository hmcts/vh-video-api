using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers;
using VideoApi.Events.Handlers.Core;
using VideoApi.Services;

namespace VideoApi.UnitTests.Events
{
    public abstract class EventHandlerTestBase
    {
        protected Mock<ICommandHandler> CommandHandlerMock;
        protected List<IEventHandler> EventHandlersList;
        protected Mock<IQueryHandler> QueryHandlerMock;
        protected Mock<IConsultationCache> ConsultationCacheMock;
        protected Mock<IMemoryCache> MemoryCacheMock;
        protected Mock<IRoomReservationService> RoomReservationServiceMock;
        

        protected Conference TestConference;

        [SetUp]
        public void Setup()
        {
            QueryHandlerMock = new Mock<IQueryHandler>();
            CommandHandlerMock = new Mock<ICommandHandler>();
            ConsultationCacheMock = new Mock<IConsultationCache>();
            MemoryCacheMock = new Mock<IMemoryCache>();
            RoomReservationServiceMock = new Mock<IRoomReservationService>();

            EventHandlersList = new List<IEventHandler>
            {
                new CloseEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object),
                new DisconnectedEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object),
                new JoinedEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object),
                new JudgeAvailableEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object),
                new JudgeUnavailableEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object),

                new LeaveEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object),
                new PauseEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object),
                new SuspendEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object),
                new TransferEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object, ConsultationCacheMock.Object, RoomReservationServiceMock.Object),
                new ParticipantJoiningEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object),
                new SelfTestFailedEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object),
            };

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
        }
    }
}
