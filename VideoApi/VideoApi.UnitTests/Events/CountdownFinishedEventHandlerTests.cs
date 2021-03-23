using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers;
using VideoApi.Events.Models;
using VideoApi.Services.Contracts;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Events
{
    public class CountdownFinishedEventHandlerTests : EventHandlerTestBase<CountdownFinishedEventHandler>
    {
        [Test]
        public async Task Should_move_witness_to_waiting_room_when_countdown_is_finished()
        {
            var witnessToAdd = new ParticipantBuilder().WithUserRole(UserRole.Individual).WithHearingRole("Witness").Build();
            TestConference.Participants.Add(witnessToAdd);
            var consultationRoom = new ConsultationRoom(TestConference.Id, "ConsultationRoom1",
                VirtualCourtRoomType.Participant, false);
            var witnesses = TestConference.Participants.Where(x=> x.IsAWitness()).ToList();
            foreach (var witness in witnesses)
            {
                witness.State = ParticipantState.InConsultation;
                witness.UpdateCurrentConsultationRoom(consultationRoom);
            }
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.CountdownFinished,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = TestConference.Id,
                TimeStampUtc = DateTime.UtcNow
            };

            await _sut.HandleAsync(callbackEvent);

            _mocker.Mock<IConsultationService>()
                .Verify(
                    x => x.LeaveConsultationAsync(TestConference.Id, It.IsAny<Guid>(), consultationRoom.Label,
                        RoomType.WaitingRoom.ToString()), Times.AtLeast(1));
            foreach (var witness in witnesses)
            {
                _mocker.Mock<IConsultationService>()
                    .Verify(
                        x => x.LeaveConsultationAsync(TestConference.Id, witness.Id, consultationRoom.Label,
                            RoomType.WaitingRoom.ToString()), Times.Once());
            }
            
        }
    }
}
