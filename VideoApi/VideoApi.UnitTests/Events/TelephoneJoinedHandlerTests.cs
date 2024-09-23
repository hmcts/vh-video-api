using System;
using System.Threading.Tasks;
using Moq;
using VideoApi.DAL.Commands;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers;
using VideoApi.Events.Models;
using VideoApi.Services;
using VideoApi.Services.Contracts;

namespace VideoApi.UnitTests.Events;

public class TelephoneJoinedHandlerTests : EventHandlerTestBase<TelephoneJoinedEventHandler>
{
    private Mock<IVideoPlatformService> _videoPlatformServiceMock;
    private Mock<ISupplierPlatformServiceFactory> _supplierPlatformServiceFactoryMock;
    
    [SetUp]
    public void SetUp()
    {
        _videoPlatformServiceMock = _mocker.Mock<IVideoPlatformService>();
        _supplierPlatformServiceFactoryMock = _mocker.Mock<ISupplierPlatformServiceFactory>();
        _supplierPlatformServiceFactoryMock.Setup(x => x.Create(It.IsAny<Supplier>())).Returns(_videoPlatformServiceMock.Object);
    }
    
    [Test]
    public async Task Should_update_participant_status_to_connected()
    {
        var conference = TestConference;
        var telephoneId = Guid.NewGuid();

        var callbackEvent = new CallbackEvent
        {
            EventType = EventType.TelephoneJoined,
            EventId = Guid.NewGuid().ToString(),
            ConferenceId = conference.Id,
            ParticipantId = telephoneId,
            TimeStampUtc = DateTime.UtcNow,
            Phone = "01234567890",
            Reason = "Telephone User Joined"
        };
        var updateStatusCommand = new AddTelephoneParticipantCommand(conference.Id, telephoneId, "01234567890");
        CommandHandlerMock.Setup(x => x.Handle(updateStatusCommand));

        await _sut.HandleAsync(callbackEvent);

        CommandHandlerMock.Verify(
            x => x.Handle(It.Is<AddTelephoneParticipantCommand>(command =>
                command.ConferenceId == conference.Id && command.TelephoneNumber == "01234567890" &&
                command.TelephoneParticipantId == telephoneId)), Times.Once);
    }
    
    [Test]
    public async Task Should_transfer_participant_to_hearing_room_if_conference_is_in_session()
    {
        var conference = TestConference;
        conference.UpdateConferenceStatus(ConferenceState.InSession);
        var telephoneId = Guid.NewGuid();

        var callbackEvent = new CallbackEvent
        {
            EventType = EventType.TelephoneJoined,
            EventId = Guid.NewGuid().ToString(),
            ConferenceId = conference.Id,
            ParticipantId = telephoneId,
            TimeStampUtc = DateTime.UtcNow,
            Phone = "01234567890",
            Reason = "Telephone User Joined"
        };
        var updateStatusCommand = new AddTelephoneParticipantCommand(conference.Id, telephoneId, "01234567890");
        CommandHandlerMock.Setup(x => x.Handle(updateStatusCommand));

        await _sut.HandleAsync(callbackEvent);

        CommandHandlerMock.Verify(
            x => x.Handle(It.Is<AddTelephoneParticipantCommand>(command =>
                command.ConferenceId == conference.Id && command.TelephoneNumber == "01234567890" &&
                command.TelephoneParticipantId == telephoneId)), Times.Once);
        
        _videoPlatformServiceMock.Verify(
            x => x.TransferParticipantAsync(conference.Id, telephoneId.ToString(), RoomType.WaitingRoom.ToString(),
                RoomType.HearingRoom.ToString()), Times.Once);
        VerifySupplierUsed(TestConference.Supplier, Times.Exactly(1));
    }
}
