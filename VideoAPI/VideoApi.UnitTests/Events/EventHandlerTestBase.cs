using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework;
using VideoApi.DAL.Queries.Core;
using VideoApi.Events.Handlers;
using VideoApi.Events.Hub;
using VideoApi.Events.ServiceBus;
using VideoApi.UnitTests.Stubs;

namespace VideoApi.UnitTests.Events
{
    public abstract class EventHandlerTestBase
    {
        protected Mock<IQueryHandler> QueryHandlerMock;
        protected IServiceBusQueueClient ServiceBusQueueClient;
        protected Mock<IHubContext<EventHub, IEventHubClient>> EventHubContextMock;
        protected Mock<IEventHubClient> EventHubClientMock;
        protected List<IEventHandler> EventHandlersList;
        
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
            
        }
    }
}