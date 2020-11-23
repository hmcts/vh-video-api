using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Extras.Moq;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers;
using VideoApi.Events.Handlers.Core;

namespace VideoApi.UnitTests.Events
{
    public class EventHandlerFactoryTests
    {
        private AutoMock _mocker;

        private EventHandlerFactory _sut;

        [SetUp]
        public void Setup()
        {
            _mocker = AutoMock.GetLoose();
            var eventHandlers = new IEventHandler[] {
                    _mocker.Create<CloseEventHandler>(),
                    _mocker.Create<DisconnectedEventHandler>(),
                    _mocker.Create<JoinedEventHandler>(),
                    _mocker.Create<EndpointJoinedEventHandler>(),
                    _mocker.Create<EndpointTransferredEventHandler>(),
                    _mocker.Create<EndpointDisconnectedEventHandler>(),
                    _mocker.Create<LeaveEventHandler>(),
                    _mocker.Create<StartEventHandler>(),
                    _mocker.Create<PauseEventHandler>(),
                    _mocker.Create<SuspendEventHandler>(),
                    _mocker.Create<TransferEventHandler>(),
                    _mocker.Create<ParticipantJoiningEventHandler>(),
                    _mocker.Create<SelfTestFailedEventHandler>(),
                };
            _sut = _mocker.Create<EventHandlerFactory>(new TypedParameter(typeof(IEnumerable<IEventHandler>), eventHandlers));
        }
        
        [TestCase(EventType.Pause, typeof(PauseEventHandler))]
        [TestCase(EventType.Joined, typeof(JoinedEventHandler))]
        [TestCase(EventType.Close, typeof(CloseEventHandler))]
        [TestCase(EventType.Disconnected, typeof(DisconnectedEventHandler))]
        [TestCase(EventType.Leave, typeof(LeaveEventHandler))]
        [TestCase(EventType.Transfer, typeof(TransferEventHandler))]
        [TestCase(EventType.ParticipantJoining, typeof(ParticipantJoiningEventHandler))]
        [TestCase(EventType.SelfTestFailed, typeof(SelfTestFailedEventHandler))]
        [TestCase(EventType.Start, typeof(StartEventHandler))]
        [TestCase(EventType.EndpointJoined, typeof(EndpointJoinedEventHandler))]
        [TestCase(EventType.EndpointDisconnected, typeof(EndpointDisconnectedEventHandler))]
        [TestCase(EventType.EndpointTransfer, typeof(EndpointTransferredEventHandler))]
        public void Should_return_instance_of_event_handler_when_factory_get_is_called_with_valid_request(
            EventType eventType, Type typeOfEventHandler)
        {
            var eventHandler = _sut.Get(eventType);
            eventHandler.Should().BeOfType(typeOfEventHandler);
        }

        [Test]
        public void Should_throw_exception_when_event_type_is_not_supported()
        {
            Action action = () => _sut.Get(EventType.None);
            action.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
