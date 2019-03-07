using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Hub;
using VideoApi.UnitTests.Stubs;

namespace VideoApi.UnitTests.Events
{
    public abstract class EventHandlerTestBase
    {
        protected Mock<IQueryHandler> QueryHandlerMock;
        protected ServiceBusQueueClientStub ServiceBusQueueClient;
        protected Mock<IHubContext<EventHub, IEventHubClient>> EventHubContextMock;
        protected Mock<IEventHubClient> EventHubClientMock;
        protected List<IEventHandler> EventHandlersList;

        protected Conference TestConference;
        
        [SetUp]
        public void Setup()
        {
            QueryHandlerMock = new Mock<IQueryHandler>();
            ServiceBusQueueClient = new ServiceBusQueueClientStub();
            EventHubContextMock = new Mock<IHubContext<EventHub, IEventHubClient>>();
            EventHubClientMock = new Mock<IEventHubClient>();

            EventHandlersList = new List<IEventHandler>
            {
                new CloseEventHandler(QueryHandlerMock.Object, ServiceBusQueueClient, EventHubContextMock.Object),
                new DisconnectedEventHandler(QueryHandlerMock.Object, ServiceBusQueueClient, EventHubContextMock.Object),
                new HelpEventHandler(QueryHandlerMock.Object, ServiceBusQueueClient, EventHubContextMock.Object),
                new JoinedEventHandler(QueryHandlerMock.Object, ServiceBusQueueClient, EventHubContextMock.Object),
                new JudgeAvailableEventHandler(QueryHandlerMock.Object, ServiceBusQueueClient, EventHubContextMock.Object),
                new LeaveEventHandler(QueryHandlerMock.Object, ServiceBusQueueClient, EventHubContextMock.Object),
                new PauseEventHandler(QueryHandlerMock.Object, ServiceBusQueueClient, EventHubContextMock.Object),
                new TransferEventHandler(QueryHandlerMock.Object, ServiceBusQueueClient, EventHubContextMock.Object)
            };

            TestConference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.VideoHearingsOfficer, null)
                .WithParticipant(UserRole.Individual, "Claimant")
                .WithParticipant(UserRole.Representative, "Claimant")
                .WithParticipant(UserRole.Individual, "Defendant")
                .WithParticipant(UserRole.Representative, "Defendant")
                .Build();
            
            QueryHandlerMock
                .Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(TestConference);
            
            foreach (var participant in TestConference.GetParticipants())
            {
                EventHubContextMock.Setup(x => x.Clients.Group(participant.Username.ToString()))
                    .Returns(EventHubClientMock.Object);
            }
        }
    }
}