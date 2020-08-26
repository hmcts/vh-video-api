using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
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
using VideoApi.Services.Contracts;

namespace VideoApi.UnitTests.Events
{
    public abstract class EventHandlerTestBase
    {
        protected Mock<ICommandHandler> CommandHandlerMock;
        protected List<IEventHandler> EventHandlersList;
        protected Mock<IQueryHandler> QueryHandlerMock;
        protected IRoomReservationService RoomReservationService;
        private Mock<ILogger<IRoomReservationService>> _loggerRoomReservationMock;
        private IMemoryCache _memoryCache;

        protected Conference TestConference;

        [SetUp]
        public void Setup()
        {
            QueryHandlerMock = new Mock<IQueryHandler>();
            CommandHandlerMock = new Mock<ICommandHandler>();
            _loggerRoomReservationMock = new Mock<ILogger<IRoomReservationService>>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            RoomReservationService = new RoomReservationService(_memoryCache, _loggerRoomReservationMock.Object);

            EventHandlersList = new List<IEventHandler>
            {
                new CloseEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object),
                new DisconnectedEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object),
                new JoinedEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object),
                new EndpointJoinedEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object),
                new LeaveEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object),
                new StartEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object),
                new CountdownFinishedEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object),
                new PauseEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object),
                new SuspendEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object),
                new TransferEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object, RoomReservationService),
                new ParticipantJoiningEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object),
                new SelfTestFailedEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object),
            };

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
