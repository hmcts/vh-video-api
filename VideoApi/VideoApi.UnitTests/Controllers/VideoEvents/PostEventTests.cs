using System;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Moq;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using VideoApi.Controllers;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;
using VHEventType = VideoApi.Domain.Enums.EventType;

namespace VideoApi.UnitTests.Controllers.VideoEvents;

public class PostEventTests
{
    private AutoMock _mocker;
    
    private VideoEventsController _sut;
    
    [SetUp]
    public void Setup()
    {
        _mocker = AutoMock.GetLoose();
        
        _sut = _mocker.Create<VideoEventsController>();
        
        _mocker.Mock<IEventHandlerFactory>().Setup(x => x.Get(It.IsAny<VHEventType>())).Returns(_mocker.Mock<IEventHandler>().Object);
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
    [TestCase(EventType.TelephoneJoined, true)]
    [TestCase(EventType.TelephoneTransfer, true)]
    [TestCase(EventType.TelephoneDisconnected, true)]
    public async Task PostEventAsync_FireEvent(EventType eventType, bool shouldHandleEvent)
    {
        // Arrange
        var conferenceEventRequest = new ConferenceEventRequest
        {
            EventType = eventType
        };
        
        // Act
        await _sut.PostEventAsync(conferenceEventRequest);
        
        // Assert
        _mocker.Mock<ICommandHandler>().Verify(x => x.Handle(It.IsAny<SaveEventCommand>()), Times.Once);
        _mocker.Mock<IEventHandlerFactory>().Verify(x => x.Get(It.IsAny<VHEventType>()), shouldHandleEvent ? Times.Once() : Times.Never());
        _mocker.Mock<IEventHandler>().Verify(x => x.HandleAsync(It.IsAny<CallbackEvent>()), shouldHandleEvent ? Times.Once() : Times.Never());
    }
    
    // Covers scenario highlighted in https://tools.hmcts.net/jira/browse/VIH-11170
    [Test]
    public async Task PostEventAsync_EdgeCase()
    {
        // Arrange
        var conferenceEventRequest = new ConferenceEventRequest
        {
            EventId = Guid.NewGuid().ToString(),
            EventType = EventType.RoomParticipantTransfer,
            TimeStampUtc = DateTime.UtcNow,
            ConferenceId = Guid.NewGuid().ToString(),
            ParticipantId = Guid.NewGuid().ToString(),
            ParticipantRoomId = "16036",
            TransferFrom = "WaitingRoom",
            TransferTo = "JudgeJOHConsultationRoom1",
            Reason = "",
            Phone = ""
        };
        
        // Act
        await _sut.PostEventAsync(conferenceEventRequest);
        
        // Assert
        _mocker.Mock<IEventHandlerFactory>().Verify(x => x.Get(VHEventType.RoomParticipantTransfer), Times.Once);
        
        _mocker.Mock<ICommandHandler>().Verify(x => x.Handle(It.Is<SaveEventCommand>(cmd => 
            cmd.ConferenceId == Guid.Parse(conferenceEventRequest.ConferenceId) &&
            cmd.ExternalEventId == conferenceEventRequest.EventId &&
            cmd.EventType ==  VHEventType.RoomParticipantTransfer &&
            cmd.ExternalTimestamp == conferenceEventRequest.TimeStampUtc &&
            cmd.TransferredFrom == (VideoApi.Domain.Enums.RoomType?)RoomType.WaitingRoom &&
            cmd.TransferredTo == (VideoApi.Domain.Enums.RoomType?)RoomType.ConsultationRoom &&
            cmd.Reason == conferenceEventRequest.Reason &&
            cmd.Phone == conferenceEventRequest.Phone)), Times.Once);
        
        _mocker.Mock<IEventHandler>().Verify(x => x.HandleAsync(It.Is<CallbackEvent>(cbe => 
            cbe.EventId == conferenceEventRequest.EventId && 
            cbe.EventType == VHEventType.RoomParticipantTransfer &&
            cbe.ConferenceId == Guid.Parse(conferenceEventRequest.ConferenceId) &&
            cbe.Reason == conferenceEventRequest.Reason &&
            cbe.TransferTo == VideoApi.Domain.Enums.RoomType.ConsultationRoom &&
            cbe.TransferFrom == VideoApi.Domain.Enums.RoomType.WaitingRoom &&
            cbe.TimeStampUtc == conferenceEventRequest.TimeStampUtc &&
            cbe.ParticipantId == Guid.Parse(conferenceEventRequest.ParticipantId) &&
            cbe.Phone == conferenceEventRequest.Phone &&
            cbe.TransferredFromRoomLabel == conferenceEventRequest.TransferFrom &&
            cbe.TransferredToRoomLabel == conferenceEventRequest.TransferTo &&
            cbe.ParticipantRoomId == 16036)), Times.Once);
    }
}
