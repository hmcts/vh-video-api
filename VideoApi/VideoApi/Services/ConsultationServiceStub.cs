using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services.Contracts;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Services
{
    [ExcludeFromCodeCoverage]
    public class ConsultationServiceStub : IConsultationService
    {
        public Task<Room> GetAvailableConsultationRoomAsync(Guid conferenceId, VirtualCourtRoomType roomType)
        {
            var room = new Room(Guid.NewGuid(), "Judge", VirtualCourtRoomType.JudgeJOH);
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
