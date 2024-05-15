using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using VideoApi.DAL.Commands;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers;
using VideoApi.Events.Models;

namespace VideoApi.UnitTests.Events
{
    public class EndpointTransferredEventHandlerTests : EventHandlerTestBase<EndpointTransferredEventHandler>
    {        
        [TestCase(RoomType.WaitingRoom, RoomType.HearingRoom, EndpointState.Connected)]
        [TestCase(RoomType.HearingRoom, RoomType.WaitingRoom, EndpointState.Connected)]
        [TestCase(RoomType.ConsultationRoom, RoomType.WaitingRoom, EndpointState.Connected)]
        [TestCase(RoomType.ConsultationRoom, RoomType.HearingRoom, EndpointState.Connected)]
        [TestCase(RoomType.WaitingRoom, RoomType.ConsultationRoom, EndpointState.InConsultation)]
        public async Task Should_send_participant_status_messages_to_clients_and_asb_when_transfer_occurs(RoomType from, RoomType to, EndpointState status)
        {
            var conference = TestConference;
            var endpointForEvent = conference.GetEndpoints().First();
            
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.EndpointTransfer,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id,
                ParticipantId = endpointForEvent.Id,
                TransferFrom = from,
                TransferTo = to,
                TransferredFromRoomLabel = from.ToString(),
                TransferredToRoomLabel = to.ToString(),
                TimeStampUtc = DateTime.UtcNow
            };
            await _sut.HandleAsync(callbackEvent);

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateEndpointStatusAndRoomCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.EndpointId == endpointForEvent.Id &&
                    command.Status == status &&
                    command.Room == to)), Times.Once);
        }
    }
}
