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
        public Task<ConsultationRoom> CreateNewConsultationRoomAsync(Guid conferenceId, VirtualCourtRoomType roomType = VirtualCourtRoomType.Participant, bool locked = true)
        {
            var room = new ConsultationRoom(Guid.NewGuid(), "Label", roomType, locked);
            return Task.FromResult(room);
        }

        public Task<ConsultationRoom> GetAvailableConsultationRoomAsync(Guid conferenceId, VirtualCourtRoomType roomType)
        {
            var room = new ConsultationRoom(Guid.NewGuid(), "Judge", VirtualCourtRoomType.JudgeJOH, false);
            return Task.FromResult(room);
        }

        public Task EndpointTransferToRoomAsync(Guid conferenceId, Guid endpointId, string room)
        {
            return Task.CompletedTask;
        }

        public Task ParticipantTransferToRoomAsync(Guid conferenceId, Guid participantId, string room)
        {
            return Task.CompletedTask;
        }

        public Task LeaveConsultationAsync(Guid conferenceId, Guid participantId, string fromRoom, string toRoom)
        {
            return Task.CompletedTask;
        }
    }
}
