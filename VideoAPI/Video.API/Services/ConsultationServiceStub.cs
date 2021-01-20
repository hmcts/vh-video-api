using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services.Contracts;
using Task = System.Threading.Tasks.Task;

namespace Video.API.Services
{
    [ExcludeFromCodeCoverage]
    public class ConsultationServiceStub : IConsultationService
    {
        public Task<Room> CreateNewConsultationRoomAsync(Guid conferenceId, VirtualCourtRoomType roomType = VirtualCourtRoomType.Participant, bool locked = false)
        {
            var room = new Room(Guid.NewGuid(), "Label", roomType, locked);
            return Task.FromResult(room);
        }

        public Task<Room> GetAvailableConsultationRoomAsync(Guid conferenceId, VirtualCourtRoomType roomType)
        {
            var room = new Room(Guid.NewGuid(), "Judge", VirtualCourtRoomType.JudgeJOH, false);
            return Task.FromResult(room);
        }

        public Task JoinConsultationRoomAsync(Guid conferenceId, Guid participantId, string room)
        {
            return Task.CompletedTask;
        }

        public Task LeaveConsultationAsync(Guid conferenceId, Guid participantId, string fromRoom, string toRoom)
        {
            return Task.CompletedTask;
        }
    }
}
