using Autofac.Extras.Moq;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using VideoApi.Controllers;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;
using VHEventType = VideoApi.Domain.Enums.EventType;

namespace VideoApi.UnitTests.Controllers.VideoEvents
{

    public class PostEventTests
    {
        private AutoMock _mocker;

        private VideoEventsController _sut;

        [SetUp]
        public void Setup()
        {
            _mocker = AutoMock.GetLoose();
            
            _sut = _mocker.Create<VideoEventsController>();
        }

        [TestCase(EventType.None, true)]
        [TestCase(EventType.Joined, true)]
        [TestCase(EventType.Disconnected, true)]
        [TestCase(EventType.Transfer, true)]
        [TestCase(EventType.Help, false)]
        [TestCase(EventType.Start, true)]
        [TestCase(EventType.CountdownFinished, true)]
        [TestCase(EventType.Pause, true)]
        [TestCase(EventType.Close, true)]
        [TestCase(EventType.Leave, true)]
        [TestCase(EventType.Consultation, true)]
        [TestCase(EventType.MediaPermissionDenied, true)]
        [TestCase(EventType.ParticipantJoining, true)]
        [TestCase(EventType.SelfTestFailed, true)]
        [TestCase(EventType.Suspend, true)]
        [TestCase(EventType.VhoCall, true)]
        [TestCase(EventType.ParticipantNotSignedIn, true)]
        [TestCase(EventType.EndpointJoined, true)]
        [TestCase(EventType.EndpointDisconnected, true)]
        [TestCase(EventType.EndpointTransfer, true)]
        [TestCase(EventType.ConnectingToEventHub, false)]
        [TestCase(EventType.SelectingMedia, false)]
        [TestCase(EventType.ConnectingToConference, false)]
        [TestCase(EventType.RoomParticipantJoined, true)]
        [TestCase(EventType.RoomParticipantDisconnected, true)]
        [TestCase(EventType.RoomParticipantTransfer, true)]
        public async Task PostEventAsync_FireEvent(EventType eventType, bool shouldHandleEvent)
        {
            // Arrange
            var conferenceEventRequest = new ConferenceEventRequest
            {
                EventType = eventType
            };

            _mocker.Mock<IEventHandlerFactory>().Setup(x => x.Get(It.IsAny<VHEventType>())).Returns(_mocker.Mock<IEventHandler>().Object);

            // Act
            await _sut.PostEventAsync(conferenceEventRequest);

            // Assert
            _mocker.Mock<ICommandHandler>().Verify(x => x.Handle(It.IsAny<SaveEventCommand>()), Times.Once);
            _mocker.Mock<IEventHandlerFactory>().Verify(x => x.Get(It.IsAny<VHEventType>()), shouldHandleEvent ? Times.Once() : Times.Never());
            _mocker.Mock<IEventHandler>().Verify(x => x.HandleAsync(It.IsAny<CallbackEvent>()), shouldHandleEvent ? Times.Once() : Times.Never());
        }

        [Test]
        public async Task PostEventAsync_PhoneEventShouldNotFireEventHandler()
        {
            // Arrange
            var conferenceEventRequests = Enum.GetValues(typeof(EventType)).Cast<EventType>().Select(et =>
            new ConferenceEventRequest
            {
                EventType = et,
                Phone = "PhoneNumber"
            });

            _mocker.Mock<IEventHandlerFactory>().Setup(x => x.Get(It.IsAny<VHEventType>())).Returns(_mocker.Mock<IEventHandler>().Object);

            foreach (var conferenceEventRequest in conferenceEventRequests)
            {
                // Act
                await _sut.PostEventAsync(conferenceEventRequest);

                // Assert
                _mocker.Mock<IEventHandlerFactory>().Verify(x => x.Get(It.IsAny<VHEventType>()), Times.Never);
                _mocker.Mock<IEventHandler>().Verify(x => x.HandleAsync(It.IsAny<CallbackEvent>()), Times.Never);
            }
        }
    }
}
